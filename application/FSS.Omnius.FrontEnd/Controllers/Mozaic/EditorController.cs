using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.FrontEnd.Controllers.Mozaic
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
    public class EditorController : Controller
    {
        // GET: Editor
        public ActionResult Index(FormCollection formParams)
        {
            COREobject core = COREobject.i;
            DBEntities context = core.Context;
            if (formParams["appId"] != null)
            {
                int appId = int.Parse(formParams["appId"]);
                core.User.DesignAppId = appId;
                context.SaveChanges();
                ViewData["appId"] = appId;
                ViewData["pageId"] = formParams["pageId"];
                ViewData["appName"] = context.Applications.Find(appId).DisplayName;
            }
            else
            {
                var userApp = core.User.DesignApp;
                if (userApp == null)
                    userApp = context.Applications.First();
                ViewData["appId"] = userApp.Id;
                ViewData["pageId"] = 0;
                ViewData["appName"] = userApp.DisplayName;
            }

            return View();
        }
    }
}
