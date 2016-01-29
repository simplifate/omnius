using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    [PersonaAuthorize(Roles = "Admin", Module = "CORE")]
    public class PortalController : Controller
    {
        // GET: Portal
        public ActionResult Index()
        {
            Modules.CORE.CORE core = new Modules.CORE.CORE();
            ViewBag.loggedUserCount = core.Persona.getLoggedCount();

            return View();
        }
        public ActionResult ModuleAdmin()
        {
            return View();
        }
        public ActionResult UsersOnline()
        {
            return View();
        }
        public ActionResult ActiveProfile()
        {
            return View();
        }
        public ActionResult AppValidation()
        {
            return View();
        }
    }
}
