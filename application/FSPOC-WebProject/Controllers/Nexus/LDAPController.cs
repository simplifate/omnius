using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;
using FSS.Omnius.Entitron.Entity.Nexus;

namespace FSS.Omnius.Controllers.Nexus
{
    public class LDAPController : Controller
    {
        // GET: LDAP
        public ActionResult Index()
        {

            DBEntities e = new DBEntities();
				return View(e.Ldaps);
        }

        public ActionResult Create()
        {
            return View("~/Views/Nexus/LDAP/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Create(Ldap model)
        {
            // Záznam již existuje a proto nepožadujeme heslo
            if(!model.Id.Equals(null))
            {
                ModelState["Bind_Password"].Errors.Clear();
            }

            if(ModelState.IsValid)
            {
                DBEntities e = new DBEntities();
                e.Ldaps.Add(model);
                e.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return PartialView("~/Views/Nexus/LDAP/Form.cshtml");
            }
        }
    }
}