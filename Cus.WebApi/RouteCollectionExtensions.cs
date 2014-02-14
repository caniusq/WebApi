using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Cus.WebApi
{
    /// <summary>
    /// WebApi路由扩展
    /// </summary>
    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// 注册WebApi路由
        /// </summary>
        /// <param name="routes">路由集合</param>
        /// <param name="name">路由名称</param>
        /// <param name="urlBase">路由基路径</param>
        public static void MapWebApiRoute(this RouteCollection routes, string name, string urlBase = "api")
        {
            var dic = new RouteValueDictionary(new
            {
                controller = UrlParameter.Optional,
                action = UrlParameter.Optional
            });
            routes.Add(name, new Route(urlBase + "/{controller}/{action}", dic, new ApiRouteHandler()));
        }
    }
}
