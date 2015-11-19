using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;

namespace FSPOC_WebProject.Controllers.Nexus
{
    public class LDAPController : Controller
    {
        // GET: LDAP
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();

            return View("../Nexus/LDAP/Index", e.Ldaps);
        }

        public ActionResult Create()
        {
            return View("../Nexus/LDAP/Form");
        }
    }
}