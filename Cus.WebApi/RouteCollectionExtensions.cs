using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Cus.WebApi
{
    public static class RouteCollectionExtensions
    {
        public static void MapWebApiRoute(this RouteCollection routes, string name)
        {
            var dic = new RouteValueDictionary(new
            {
                category = UrlParameter.Optional,
                action = UrlParameter.Optional
            });
            routes.Add(name, new Route("api/{handler}/{method}", dic, new ApiRouteHandler()));
        }
    }
}
