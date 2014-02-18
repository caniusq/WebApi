using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Cus.WebApi
{
    class ApiHandler : IHttpHandler, IRequiresSessionState
    {
        private static Encoding _encoding = new UTF8Encoding(false);

        private readonly ApiController _api;
        public ApiHandler(ApiController api)
        {
            _api = api;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (_api == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            string action = (string)context.Items["action"];
            bool queryUser = "special-user".Equals(action);

            context.Response.ContentEncoding = _encoding;
            context.Response.ContentType = "application/json";

            var apiManager = ApiManager.GetOrCreate(_api.GetType());

            if (!queryUser && !string.IsNullOrEmpty(action))
            {
                apiManager.InvokeWebMethod(context, _api, action);
            }
            else
            {
                if (!ApiManager.IsDebug)
                {
                    context.Response.StatusCode = 401;
                }
                else if (apiManager.EnableDocumentation())
                {
                    if (queryUser)
                    {
                        apiManager.InvokeReturnUser(context, _api.User);
                    }
                    else
                    {
                        if ("GET".Equals(context.Request.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ResManager.ProcessRes(context, "documentation.html");
                        }
                        else if ("POST".Equals(context.Request.HttpMethod, StringComparison.CurrentCultureIgnoreCase))
                        {
                            context.Response.Write(JsonConvert.SerializeObject(apiManager.ApiDescriptor, Formatting.Indented));
                        }
                    }
                }
                else
                {
                    string returnString = "没有可用的文档，请定义Documentation特性。";
                    context.Response.Write(returnString);
                }
            }
        }
    }
}
