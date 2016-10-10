﻿using FSPOC_WebProject.Views;
using FSS.Omnius.Controllers.CORE;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Net.Mail;
using System.Net;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Controllers.Tapestry;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace FSPOC_WebProject
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
            App_Start.AppStart.AppInitialize();
            Logger.Log.Info("Omnius starts");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                List<string> bodyLines = new List<string>();
                bodyLines.Add($"URL: {Request.Url.AbsoluteUri}");
                bodyLines.Add($"Method: {Request.HttpMethod}");
                bodyLines.Add($"Current User: {Context.User?.Identity?.Name}");
            
                bodyLines.Add("POST data:");
                NameValueCollection form = Request.Unvalidated.Form;
                foreach (string key in form.AllKeys)
                {
                    bodyLines.Add($"{key} => {Server.HtmlEncode(form[key])}");
                }
                    
                bodyLines.Add("");
                bodyLines.Add("Errors:");
                foreach (var error in Context.AllErrors)
                {
                    var curError = error;
                    while (curError != null)
                    {
                        bodyLines.Add($"Message: {curError.Message}");
                        bodyLines.Add($"Method: {curError.TargetSite.ToString()}");
                        bodyLines.Add($"Error type: {curError.GetType()}");
                        bodyLines.Add($"Trace: {curError.StackTrace}");

                        // validation error
                        if (curError?.GetType() == typeof(DbEntityValidationException))
                        {
                            bodyLines.Add($"Validation errors:");
                            foreach (DbEntityValidationResult valE in (curError as DbEntityValidationException).EntityValidationErrors)
                            {
                                foreach (var validationMessage in valE.ValidationErrors)
                                {
                                    bodyLines.Add($" -> {validationMessage.PropertyName}: {validationMessage.ErrorMessage}");
                                }
                            }
                        }

                        bodyLines.Add("");
                        // inner error
                        curError = curError.InnerException;
                    }
                }

                Logger.Log.Fatal(string.Join(Environment.NewLine, bodyLines));
            }
            catch (Exception exc)
            {
                Logger.Log.Error($"!Failed to log error!:{Environment.NewLine}{exc.Message}");
            }
        }

        protected void Application_BeginRequest()
        {
            RunController.requestStart = DateTime.Now;
            DBEntities.Create();
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
            DBEntities.Destroy();
        }
    }
}
