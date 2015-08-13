using System.Web.Mvc;
using System.Web.Routing;

namespace FSSWorkflowDesigner
{
    public class Application : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();
        }
        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}