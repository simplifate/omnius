using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;

namespace FSS.Omnius.Controllers.Nexus
{
    public class LDAPController : Controller
    {
        // GET: LDAP
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/LDAP/Index", e.Ldaps);
        }

        public ActionResult Create()
        {
            return View("~/Views/Nexus/LDAP/Form");
        }
    }
}