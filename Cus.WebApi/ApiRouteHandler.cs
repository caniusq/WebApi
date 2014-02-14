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

        private static Dictionary<string, Type> _handlerDic = new Dictionary<string, Type>();
        private static Type FindHandlerType(string handlerName, HttpApplication app)
        {
            Type handlerType;
            if (_handlerDic.TryGetValue(handlerName, out handlerType)) return handlerType;
            lock (_syncRoot)
            {
                if (_handlerDic.TryGetValue(handlerName, out handlerType)) return handlerType;

                var asm = GetGlobalAssembly(app);

                foreach (Type type in asm.GetTypes())
                {
                    if (handlerName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!type.IsSubclassOf(typeof(ApiHandler))) continue;
                        handlerType = type;
                        break;
                    }
                }
                if (handlerType == null) throw new ApplicationException(handlerName + " WebApi Not Found.");
                _handlerDic.Add(handlerName, handlerType);
                return handlerType;
            }
        }

        public System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            object handler = requestContext.RouteData.Values["handler"];
            object method = requestContext.RouteData.Values["method"];

            if (handler == UrlParameter.Optional)
            {
                throw new NotImplementedException();
            }

            if ("special.res".Equals(handler))
            {
                requestContext.HttpContext.Items["res"] = (string)method;
                return new ResHandler();
            }

            if (method != UrlParameter.Optional)
            {
                requestContext.HttpContext.Items["method"] = (string)method;
            }
            Type type = FindHandlerType((string)handler, requestContext.HttpContext.ApplicationInstance);
            return (ApiHandler)Activator.CreateInstance(type);
        }
    }
}
