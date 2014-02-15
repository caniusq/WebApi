using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using System.Configuration;
using System.Web.Configuration;

namespace Cus.WebApi
{
    /// <summary>
    /// 接口管理器
    /// </summary>
    class ApiManager
    {
        const string DOCUMENTATION_FORM_KEY = "C35A353FD50A49E3A4DDF3E2B1DD04D5";
        private static bool? _isDebug;
        private static Dictionary<Type, ApiManager> _cache = new Dictionary<Type, ApiManager>();

        public static ApiManager GetOrCreate(Type type)
        {
            ApiManager manager;
            if (!_cache.TryGetValue(type, out manager))
            {
                lock (_cache)
                {
                    if (!_cache.TryGetValue(type, out manager))
                    {
                        manager = new ApiManager(type);
                        _cache.Add(type, manager);
                    }
                }
            }
            return manager;
        }

        public static bool IsDebug
        {
            get
            {
                if (_isDebug == null)
                {
                    var section = (CompilationSection)ConfigurationManager.GetSection("system.web/compilation");
                    _isDebug = section == null ? false : section.Debug;
                }
                return _isDebug.Value;
            }
        }

        private readonly ApiDescriptor _apiDescriptor;
        private bool _documentaionEnabled;
        private object _syncRoot = new object();

        private ApiManager(Type type)
        {
            var container = new TypeDefinitionContainer(true);
            _apiDescriptor = new ApiDescriptor(type);
            _apiDescriptor.Container = container;
            _apiDescriptor.ScanTypeDefinitions();
        }

        public ApiDescriptor ApiDescriptor
        {
            get
            {
                return _apiDescriptor;
            }
        }

        /// <summary>
        /// 是否已启用文档功能
        /// </summary>
        public bool DocumentaionEnabled
        {
            get
            {
                return _documentaionEnabled;
            }
        }

        public void InvokeReturnUser(HttpContext context, Identity user)
        {
            string retString = JsonConvert.SerializeObject(new Response { User = user }, Formatting.Indented);

            if (!string.IsNullOrEmpty(retString))
            {
                context.Response.Write(retString);
            }
        }

        public void InvokeWebMethod(HttpContext context, ApiController api, string methodName)
        {
            var result = PrepareMethod(context, api, methodName);

            if (result.Response.code == ApiException.CODE_SUCCESS)
            {
                string inputString = null;
                List<object> paraList = null;
                if (context.Request.Form.Count > 0)
                {
                    string val = context.Request.Form.Get(DOCUMENTATION_FORM_KEY);
                    if(!string.IsNullOrEmpty(val)) inputString = val;
                }
                else if (context.Request.ContentLength != 0)
                {
                    var buffer = new byte[4096];
                    using (var ms = new MemoryStream())
                    {
                        while (true)
                        {
                            int len = context.Request.InputStream.Read(buffer, 0, buffer.Length);
                            if (len == 0) break;
                            ms.Write(buffer, 0, len);
                        }

                        inputString = context.Request.ContentEncoding.GetString(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
                    }
                }

                InvokeMethod(context, result, api, inputString, paraList);
            }

            string retString = JsonConvert.SerializeObject(result.Response, Formatting.Indented);

            if (!string.IsNullOrEmpty(retString))
            {
                context.Response.Write(retString);
            }
        }

        private PrepareRusult PrepareMethod(HttpContext context, ApiController api, string methodName)
        {
            var result = new PrepareRusult();

            MethodDescriptor method;
            if (!_apiDescriptor.MethodsDic.TryGetValue(methodName, out method))
            {
                result.Response = new Response { code = ApiException.CODE_MISS_METHOD, reason = "方法不存在" };
                return result;
            }
            result.MethodDescriptor = method;

            bool respVoid = method.ResponseParameterInfo.ParameterType == typeof(void);
            result.ResponseVoid = respVoid;

            if (respVoid)
            {
                result.Response = new Response();
            }
            else
            {
                result.Response = (Response)Activator.CreateInstance(method.ResponseParam.Type);
            }

            var resp = result.Response;
            Identity user = null;
            if (IsDebug)
            {
                string uid = context.Request.QueryString["user"];
                if (!string.IsNullOrEmpty(uid))
                {
                    user = new Identity(uid, "debug");
                    api.Authetication.SaveUser(context, user);
                }
            }

            if (user == null)
                user = api.Authetication.GetUser(context);

            if (user == null || !user.IsAuthenticated)
            {
                try
                {
                    user = api.Authetication.VerifyUser(context);
                    if (user != null)
                        api.Authetication.SaveUser(context, user);
                }
                catch (Exception ex)
                {
                    resp.code = ApiException.CODE_ERROR;
                    resp.reason = "验证时发生未知错误";
                    api.InvokeUnhandledException(context, methodName, null, ex);
                    if (ApiManager.IsDebug)
                        resp.stacktrace = ex.Message + " " + ex.StackTrace;
                    result.Response = resp;
                    return result;
                }
            }

            if (method.NeedAuth && (user == null || !user.IsAuthenticated))
            {
                resp.code = ApiException.CODE_UNAUTH;
                resp.reason = "没有权限";
                result.Response = resp;
                result.Response.redirect = context.Response.RedirectLocation;
                return result;
            }

            if (user == null)
            {
                user = new Identity();
                api.TempUser = user;
            }

            resp.User = user;
            result.Response = resp;

            return result;
        }

        public void InvokeMethod(HttpContext context, PrepareRusult prepareResult, ApiController api, string inputString, List<object> paraList = null)
        {
            var method = prepareResult.MethodDescriptor;
            var resp = prepareResult.Response;
            string methodName = method.Name;

            if (paraList == null)
            {
                paraList = new List<object>();

                JObject jobject;
                if (!string.IsNullOrEmpty(inputString))
                    jobject = (JObject)JsonConvert.DeserializeObject(inputString);
                else
                    jobject = null;

                foreach (var item in method.Params)
                {
                    try
                    {
                        JToken token;
                        if (jobject == null || !jobject.TryGetValue(item.Name, out token))
                        {
                            if (item.Def.CanNull) paraList.Add(null);
                            else
                            {
                                prepareResult.Response.code = ApiException.CODE_ARG_ERROR;
                                prepareResult.Response.reason = "缺少参数";
                                return;
                            }
                        }
                        else
                        {
                            paraList.Add(token.ToObject(item.ParameterInfo.ParameterType));
                        }
                    }
                    catch
                    {
                        prepareResult.Response.code = ApiException.CODE_ARG_ERROR;
                        prepareResult.Response.reason = string.Format("参数格式不正确:{0}", item.Name);
                        return;
                    }
                }
            }

            object ret;
            try
            {
                ret = method.MethodInfo.Invoke(api, paraList.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                var apiEx = ex.InnerException as ApiException;
                if (apiEx != null)
                {
                    resp.code = apiEx.Code;
                    resp.reason = apiEx.Message;
                }
                else if (ex.InnerException != null)
                {
                    resp.code = ApiException.CODE_ERROR;
                    resp.reason = ex.InnerException.Message;
                    api.InvokeUnhandledException(context, methodName, inputString, ex.InnerException);
                    if (ApiManager.IsDebug)
                        resp.stacktrace = ex.InnerException.Message + " " + ex.InnerException.StackTrace;
                }
                else
                {
                    resp.code = ApiException.CODE_ERROR;
                    resp.reason = string.Format("发生未知错误 method=", methodName);
                }
                return;
            }

            if (!prepareResult.ResponseVoid)
            {
                method.ResponseSetResultMethod.Invoke(resp, new object[] { ret });
            }

            if (api.TempUser != null) resp.User = api.TempUser;
                if (resp.User == null) resp.User = new Identity();
        }

        public bool EnableDocumentation()
        {
            if (_documentaionEnabled) return true;
            lock (_syncRoot)
            {
                if (_documentaionEnabled) return true;
                bool enable = false;

                var attrs = _apiDescriptor.ApiType.GetCustomAttributes(typeof(DocumentationAttribute), true);

                if (attrs.Length > 0)
                {
                    using (var fs = typeof(ApiManager).Assembly.GetManifestResourceStream(ResManager.GetResourceId("Cus.WebApi.XML")))
                    {
                        var selfProvider = new XmlDocumentationProvider(fs);
                        selfProvider.SetDocumentation(_apiDescriptor);
                    }

                    foreach (DocumentationAttribute attr in attrs)
                    {
                        string path = attr.Path;
                        if (!File.Exists(path))
                        {
                            path = HttpContext.Current.Server.MapPath(path);
                            if (!File.Exists(path)) throw new FileNotFoundException("没有找到说明文档");
                        }
                        var provider = new XmlDocumentationProvider(path);
                        provider.SetDocumentation(_apiDescriptor);
                        enable = true;
                    }

                    foreach (var method in _apiDescriptor.Methods)
                    {
                        PropertyDescriptor result;
                        if (!method.ResponseParam.PropertiesDic.TryGetValue("result", out result)) continue;
                        result.Documentation = method.ResponseParam.Documentation;
                    }
                }

                _documentaionEnabled = enable;
                return _documentaionEnabled;
            }
        }
    }

    class PrepareRusult
    {
        public Response Response { get; set; }
        public MethodDescriptor MethodDescriptor { get; set; }
        public bool ResponseVoid { get; set; }
    }
}
