using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    public class PortalController : Controller
    {
        // GET: Portal
        public ActionResult Index()
        {
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
    }
}