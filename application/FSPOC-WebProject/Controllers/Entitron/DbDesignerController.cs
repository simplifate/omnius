using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Controllers.Entitron
{
    [PersonaAuthorize(Roles = "Admin", Module = "Entitron")]
    public class DbDesignerController : Controller
    {
        // GET: DbDesigner
        public ActionResult Index(FormCollection formParams)
        {
            using (var context = new DBEntities())
            {
                int appId = 0;
                string appName = "";
                if (formParams["appId"] != null)
                {
                    appId = int.Parse(formParams["appId"]);
                    appName = context.Applications.Find(appId).DisplayName;
                }
                else
                {
                    var firstApp = context.Applications.First();
                    appId = firstApp.Id;
                    appName = firstApp.DisplayName;
                }
                ViewData["appId"] = appId;
                ViewData["appName"] = appName;

                return View();
            }
        }
    }
}