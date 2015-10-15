using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CORE
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Config CORE",
                url: "config/CORE/{action}/{id}",
                defaults: new
                {
                    controller = "Config",
                    action = "Index",
                    id = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "Config",
                url: "config/{modul}/{*url}",
                defaults: new
                {
                    controller = "Redirect",
                    action = "Config",
                    
                    url = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "Default",
                url: "{modul}/{app}/{*url}",
                defaults: new
                {
                    controller = "Redirect",
                    action = "Index",

                    modul = "Master",
                    app = UrlParameter.Optional,
                    url = UrlParameter.Optional
                });
        }
    }
}
