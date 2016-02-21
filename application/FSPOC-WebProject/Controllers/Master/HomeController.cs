using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize]
    public class HomeController : Controller
    {
        private List<Application> getAppList()
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            User currentUser = User.GetLogged(core);
            try
            {
                return core.Entitron.GetStaticTables().Applications.Where(a =>
                    a.IsPublished
                    && a.IsEnabled
                    && a.ADgroups.FirstOrDefault().ADgroup_Users.Any(adu => adu.UserId == currentUser.Id)
                ).ToList();
            }
            catch (Exception ex)
            {
                Log.Error("Error while loading application list. Exception message: " + ex.Message);
                throw ex;
            }
        }
        private void loadUserInterfaceData()
        {
            ViewData["Apps"] = getAppList();
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
    }
}
