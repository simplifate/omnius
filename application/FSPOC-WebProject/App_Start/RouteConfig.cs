using System.Web.Mvc;
using System.Web.Routing;

namespace FSPOC_WebProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // CORE
            routes.MapRoute(
                name: "CORE",
                url: "CORE/{controller}/{action}/{id}",
                defaults: new { controller = "Config", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.CORE" }
            );

            // Entitron
            routes.MapRoute(
                name: "Entitron",
                url: "Entitron/{controller}/{action}/{id}",
                defaults: new { controller = "Config", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Entitron" }
            );

            // Master
            routes.MapRoute(
                name: "Master",
                url: "Master/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Master" }
            );

            // Mozaic
            routes.MapRoute(
                name: "Mozaic",
                url: "Mozaic/{controller}/{action}/{id}",
                defaults: new { controller = "Config", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Mozaic" }
            );

            // Nexus
            routes.MapRoute(
                name: "Nexus",
                url: "Nexus/{controller}/{action}/{id}",
                defaults: new { controller = "Nexus", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Nexus" }
            );

            // Persona
            routes.MapRoute(
                name: "Persona",
                url: "Persona/{controller}/{action}/{id}",
                defaults: new { controller = "Config", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Persona" }
            );

            // Tapestry
            routes.MapRoute(
                name: "Tapestry",
                url: "Tapestry/{controller}/{action}/{id}",
                defaults: new { controller = "Builder", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Tapestry" }
            );

            // Start
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Master" }
            );
        }
    }
}

