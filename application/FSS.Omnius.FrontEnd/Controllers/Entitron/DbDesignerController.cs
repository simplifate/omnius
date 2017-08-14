using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Entitron
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Entitron")]
    public class DbDesignerController : Controller
    {
        // GET: DbDesigner
        public ActionResult Index(FormCollection formParams)
        {
            DBEntities context = DBEntities.instance;
            int appId = 0;
            string appName = "";
            if (formParams["appId"] != null)
            {
                appId = int.Parse(formParams["appId"]);
                HttpContext.GetLoggedUser().DesignAppId = appId;
                context.SaveChanges();
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
            var userId = HttpContext.GetCORE().User.Id;
            ViewData["appId"] = appId;
            ViewData["appName"] = appName;

            ViewData["currentUserId"] = userId;
            ViewData["currentUserName"] = context.Users.SingleOrDefault(u=> u.Id == userId).DisplayName;
            
            return View();
        }
    }
}
