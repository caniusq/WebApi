using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Cus.WebApi
{
    class ApiRouteHandler : IRouteHandler
    {
        private static object _syncRoot = new object();

        private static Assembly _globalAssembly;
        private static Assembly GetGlobalAssembly(HttpApplication app)
        {
            if (_globalAssembly != null) return _globalAssembly;
            lock (_syncRoot)
            {
                if (_globalAssembly != null) return _globalAssembly;
                if (app == null) throw new ApplicationException();
                Type t = app.GetType();
                while (t != null && t.Namespace == "ASP")
                {
                    t = t.BaseType;
                }
                if (t == null) throw new ApplicationException();
                _globalAssembly = Assembly.GetAssembly(t);
                return _globalAssembly;
            }
        }

        private static Dictionary<string, Type> _controllerDic = new Dictionary<string, Type>();
        private static Type FindControllerType(string controllerName, HttpApplication app)
        {
            Type controllerType;
            if (_controllerDic.TryGetValue(controllerName, out controllerType)) return controllerType;
            lock (_controllerDic)
            {
                if (_controllerDic.TryGetValue(controllerName, out controllerType)) return controllerType;

                var asm = GetGlobalAssembly(app);

                foreach (Type type in asm.GetTypes())
                {
                    if (controllerName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!type.IsSubclassOf(typeof(ApiController))) continue;
                        controllerType = type;
                        break;
                    }
                }

                if (controllerType != null)
                    _controllerDic.Add(controllerName, controllerType);

                return controllerType;
            }
        }

        private static List<Type> _controllerTypes;
        private static List<Type> FindAllControllerType(HttpApplication app)
        {
            if (_controllerTypes != null) return _controllerTypes;
            lock (_syncRoot)
            {
                if (_controllerTypes != null) return _controllerTypes;

                var asm = GetGlobalAssembly(app);
                List<Type> types = new List<Type>();

                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(ApiController))) continue;
                    types.Add(type);
                }
                _controllerTypes = types;
                return _controllerTypes;
            }
        }

        public System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            object controller = requestContext.RouteData.Values["controller"];
            object action = requestContext.RouteData.Values["action"];

            if (controller == UrlParameter.Optional)
            {
                var types = FindAllControllerType(requestContext.HttpContext.ApplicationInstance);
                return new ApiListHandler(types);
            }

            if ("special-res".Equals(controller))
            {
                requestContext.HttpContext.Items["res"] = requestContext.HttpContext.Request.QueryString["res"];
                return new ResHandler();
            }

            if (action == UrlParameter.Optional)
            {
                if (requestContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath.EndsWith("/"))
                {
                    return new ApiHandler(null);
                }
            }
            else
            {
                requestContext.HttpContext.Items["action"] = (string)action;
            }
            Type type = FindControllerType((string)controller, requestContext.HttpContext.ApplicationInstance);
            if (type == null) return new ApiHandler(null);
            return new ApiHandler((ApiController)Activator.CreateInstance(type));
        }
    }
}
