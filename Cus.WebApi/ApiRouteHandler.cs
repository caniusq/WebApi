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
            lock (_syncRoot)
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

        public System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            object controller = requestContext.RouteData.Values["controller"];
            object action = requestContext.RouteData.Values["action"];

            if (controller == UrlParameter.Optional)
            {
                throw new NotImplementedException();
            }

            if ("special.res".Equals(controller))
            {
                requestContext.HttpContext.Items["res"] = (string)action;
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
