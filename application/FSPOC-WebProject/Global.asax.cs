using FSPOC_WebProject.Views;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FSPOC_WebProject
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines        .Engines.Clear();
            ViewEngines        .Engines.Add(new MyRazorViewEngine());
            ViewEngines        .Engines.Add(new MyWebFormViewEngine());
            AreaRegistration   .RegisterAllAreas();
            UnityConfig        .RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig       .RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig        .RegisterRoutes(RouteTable.Routes);
            BundleConfig       .RegisterBundles(BundleTable.Bundles);
        }
    }
}
