using FSPOC_WebProject.Views;
using FSS.Omnius.Controllers.CORE;
using FSS.Omnius.Modules.CORE;
using System;
using System.Linq;
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
            Logger.Log         .ConfigureRootDir(Server);
            App_Start.AppStart.AppInitialize();

            Logger.Log.Info("Omnius starts");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            string body = $"URL: {Request.Url.AbsoluteUri}{Environment.NewLine}Errors:{Environment.NewLine}";
            foreach (var error in Context.AllErrors)
            {
                Logger.Log.Error(error, Request);
            }

            Logger.Log.Error(body);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!Context.Items.Contains("CORE"))
                Context.Items.Add("CORE", new CORE());
        }

        protected void Application_EndRequest()
        {
            // error
            if (new int[] { 403, 404, 500 }.Contains(Context.Response.StatusCode))
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
                rd.Values["controller"] = "Error";

                switch (Context.Response.StatusCode)
                {
                    case 403:
                        rd.Values["action"] = "UserNotAuthorized";
                        break;
                    case 404:
                        rd.Values["action"] = "PageNotFound";
                        break;
                    case 500:
                        rd.Values["action"] = "InternalServerError";
                        break;
                }

                IController c = new ErrorController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }

            (Context.Items["CORE"] as CORE).Entitron.CloseStaticTables();
        }
    }
}
