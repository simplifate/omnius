using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Configuration;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize]
    public class HomeController : Controller
    {
        private void loadUserInterfaceData()
        {
            ViewData["Apps"] = Application.getAllowed(COREobject.i.User.Id).ToList();
            ViewData["FakeAppPath"] = "/Mozaic/ViewPage/Index/9";
        }

        public ActionResult Index()
        {
            loadUserInterfaceData();
            return View();
        }
        public ActionResult Details()
        {
            loadUserInterfaceData();
            
            return View();
        }
        public ActionResult Help()
        {
            loadUserInterfaceData();
            return View();
        }

        public ActionResult RedirectToDefault()
        {
            DBEntities context = COREobject.i.Context;

            string appName = ConfigurationManager.AppSettings["DefaultApp"];
            if (context.Applications.Any(a => a.Name == appName))
                return RedirectToRoute("Run", new { appName });
            
            return RedirectToRoute("Master", new { action = "Index" });
        }
    }
}
