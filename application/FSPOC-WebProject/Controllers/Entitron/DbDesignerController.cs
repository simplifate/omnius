using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

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
                    HttpContext.GetLoggedUser().DesignAppId = appId;
                    HttpContext.GetCORE().Entitron.GetStaticTables().SaveChanges();
                    appName = context.Applications.Find(appId).DisplayName;
                }
                else
                {
                    var userApp = HttpContext.GetLoggedUser().DesignApp;
                    if (userApp == null)
                        userApp = context.Applications.First();
                    appId = userApp.Id;
                    appName = userApp.DisplayName;
                }
                ViewData["appId"] = appId;
                ViewData["appName"] = appName;

                return View();
            }
        }
    }
}