using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Routing;

namespace Cus.WebApi
{
    public class ApiRouteHandler : IRouteHandler
    {
        private static Assembly _globalAssembly;

        public System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            object handler = requestContext.RouteData.Values["handler"];
            object method = requestContext.RouteData.Values["method"];

            if (handler == UrlParameter.Optional)
            {
                throw new NotImplementedException();
            }

            if ("cus.webapi.res".Equals(handler))
            {
                requestContext.HttpContext.Items["res"] = (string)method;
                return new ResHandler();
            }

            if (method == UrlParameter.Optional)
            {
                string handlerName = (string)handler;
                var app = requestContext.HttpContext.ApplicationInstance;
                if (app == null) throw new ApplicationException();

                Type t = app.GetType();
                while (t != null && t.Namespace == "ASP")
                {
                    t = t.BaseType;
                }
                if (t == null) throw new ApplicationException();
                var asm = Assembly.GetAssembly(t);

                Type handlerType = null;
                foreach (Type type in asm.GetTypes())
                {
                    if (handlerName.Equals(type.Name))
                    {
                        if (!type.IsSubclassOf(typeof(ApiHandler))) continue;
                        handlerType = type;
                        break;
                    }
                }
                if (handlerType == null) throw new ApplicationException(handlerName + " WebApi Not Found.");

                return (ApiHandler)Activator.CreateInstance(handlerType);
            }

            throw new NotImplementedException();
        }
    }
}
