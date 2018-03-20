using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;
using FSS.Omnius.FrontEnd.Views;
using FSS.Omnius.Controllers.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Controllers.Tapestry;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using System.Web.Helpers;
using FSS.Omnius.FrontEnd.Controllers;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Text.RegularExpressions;

namespace FSS.Omnius.FrontEnd
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MyRazorViewEngine());
            //ViewEngines        .Engines.Add(new MyWebFormViewEngine());
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            Logger.Log.ConfigureRootDir(Server);
            MvcHandler.DisableMvcResponseHeader = true;
            Entitron.DefaultConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Entitron.DefaultDBType = DBCommandSet.GetSqlType(ConfigurationManager.ConnectionStrings["DefaultConnection"].ProviderName);
            App_Start.AppStart.AppInitialize();
            Logger.Log.Info("Omnius starts");

            AntiForgeryConfig.AdditionalDataProvider = new AntiforgeryStrategyOneTime();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                foreach (Exception error in Context.AllErrors)
                {
                    Omnius.Modules.Watchtower.OmniusException.Log(
                        error.Message,
                        Omnius.Modules.Watchtower.OmniusLogSource.CORE,
                        innerException: error);
                }
            }
            catch (Exception exc)
            {
                Logger.Log.Error(exc, Request);
            }
        }

        protected void Application_BeginRequest()
        {
            // SECURE: Ensure any request is returned over SSL/TLS in production
            // Has to be disabled on IIS instances without SSL certificates
            /*if (!Request.IsLocal && !Context.Request.IsSecureConnection)
            {
                var redirect = Context.Request.Url.ToString().ToLower(CultureInfo.CurrentCulture).Replace("http:", "https:");
                Response.Redirect(redirect);
            }*/

            RunController.requestStart = DateTime.Now;
            DBEntities.Create();

            // app
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            if (routeData != null)
            {
                string appName = (string)routeData.Values["appName"];
                if (string.IsNullOrEmpty(appName) && (routeData.Values["MS_SubRoutes"] as System.Web.Http.Routing.IHttpRouteData[])?.FirstOrDefault()?.Values.ContainsKey("appName") == true)
                    appName = (string)(routeData.Values["MS_SubRoutes"] as System.Web.Http.Routing.IHttpRouteData[])?.FirstOrDefault()?.Values["appName"];
                if (!string.IsNullOrEmpty(appName))
                    Entitron.Create(appName); // run controller || api run controller
                else
                {
                    int appId = routeData.Values.ContainsKey("appId")
                        ? Convert.ToInt32(routeData.Values["appId"])
                        : (routeData.Values["MS_SubRoutes"] as System.Web.Http.Routing.IHttpRouteData[])?.FirstOrDefault()?.Values.ContainsKey("appId") == true
                            ? Convert.ToInt32((routeData.Values["MS_SubRoutes"] as System.Web.Http.Routing.IHttpRouteData[])?.FirstOrDefault()?.Values["appId"] ?? -1)
                            : -1;

                    if (appId != -1)
                        Entitron.Create(appId);
                }
            }
        }

        protected void Application_PostRequestHandlerExecute()
        {
            // error
            if (new int[] { 403, 404, 500 }.Contains(Context.Response.StatusCode) && !Request.Url.AbsolutePath.StartsWith("/rest/")) {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
                rd.Values["controller"] = "Error";

                switch (Context.Response.StatusCode) {
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
                //Act as 400 and not 500 if api call has failed
                if (Context.Response.StatusCode == 500 && Context.Request.Path.Contains("/rest/")) {
                    Context.Response.StatusCode = 400;
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Bad request");
                    using (var stream = Context.Response.OutputStream)
                        stream.Write(bytes, 0, bytes.Length);
                }
                else {
                    IController c = new ErrorController();
                    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                }
            }
        }

        protected void Application_EndRequest()
        {
            if(Request.Url.AbsolutePath.ToLower().EndsWith("/core/account/login") 
                && Request.HttpMethod.ToLower() == "post"
                && Context.Response.Headers.AllKeys.Contains("Set-Cookie")
                && Context.Response.Headers["Set-Cookie"].Contains(".AspNet.ApplicationCookie")
                && Request.Form.AllKeys.Contains("UserName")
                && !string.IsNullOrEmpty(Request.Form["UserName"])
            ) {
                DBEntities db = DBEntities.instance;
                string userName = Request.Form["UserName"].ToLower();
                User user = db.Users.Single(u => u.UserName.ToLower() == userName);

                string cookies = Context.Response.Headers["Set-Cookie"];
                var m = Regex.Match(cookies, ".AspNet.ApplicationCookie=(?<AppCookie>.*?);");

                string appCookie = m.Groups["AppCookie"].Value;

                user.LastIp = Request.UserHostAddress;
                user.LastAppCookie = appCookie;

                db.SaveChanges();
            }
            
            DBEntities.Destroy();
            Entitron.Destroy();
        }
    }
}
