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
        public ActionResult Save(Ldap model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    Ldap row = e.Ldaps.Single(m => m.Id == model.Id);
                    row.Domain_Ntlm = model.Domain_Ntlm;
                    row.Domain_Kerberos = model.Domain_Kerberos;
                    row.Domain_Server = model.Domain_Server;
                    row.Bind_User = model.Bind_User;
                    row.Bind_Password = model.Bind_Password.Length > 0 ? model.Bind_Password : row.Bind_Password;
                    row.Use_SSL = model.Use_SSL;
                    row.Active = model.Active;

                    e.SaveChanges();
                }
                else
                {
                    e.Ldaps.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/LDAP/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/LDAP/Detail.cshtml", e.Ldaps.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Nexus/LDAP/Form.cshtml", e.Ldaps.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Ldap row = e.Ldaps.Single(m => m.Id == id);

            e.Ldaps.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }
    }
}