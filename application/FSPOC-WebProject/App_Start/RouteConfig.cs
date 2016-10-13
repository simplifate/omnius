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
                defaults: new { controller = "Portal", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.CORE" }
            );

            // Compass
            routes.MapRoute(
                name: "Compass",
                url: "Compass/{controller}/{action}/{id}",
                defaults: new { controller = "Compass", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Compass" }
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

            // Cortex
            routes.MapRoute(
                name: "Cortex",
                url: "Cortex/{action}/{id}",
                defaults: new { controller = "Cortex", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Cortex" }
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

            // Hermes
            routes.MapRoute(
                name: "HermesPlaceholders",
                url: "Hermes/Placeholder/{action}/{emailId}/{id}",
                defaults: new { controller = "Placeholder", action = "Index", emailId = "", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Hermes" }
            );

            routes.MapRoute(
                name: "Hermes",
                url: "Hermes/{controller}/{action}/{id}",
                defaults: new { controller = "SMTP", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Hermes" }
            );

            // Watchtower
            routes.MapRoute(
                name: "Watchtower",
                url: "Watchtower/{controller}/{action}",
                defaults: new { controller = "Log", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Watchtower" }
            );

            // RUN
            routes.MapRoute(
                name: "Run",
                url: "{appName}/{blockIdentify}",
                defaults: new { controller = "Run", action = "Index", blockIdentify = UrlParameter.Optional},
                namespaces: new string[] { "FSS.Omnius.Controllers.Tapestry" }
            );

            // Start
            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "FSS.Omnius.Controllers.Master" }
            );
        }
    }
}

