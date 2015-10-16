using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mozaic
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Config",
                url: "config/{action}/{id}",
                defaults: new
                {
                    controller = "Config",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Builder",
                url: "/builder/{app}/{action}/{id}",
                defaults: new
                {
                    controller = "Builder",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{app}/{pageId}/{tableName}/{modelId}",
                defaults: new
                {
                    controller = "Renderer",
                    action = "Show",
                    
                    tableName = UrlParameter.Optional,
                    modelId = UrlParameter.Optional
                }
            );
        }
    }
}
