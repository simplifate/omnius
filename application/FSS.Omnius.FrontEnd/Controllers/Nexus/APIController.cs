using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using System.Text;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Nexus")]
    public class APIController : Controller
    {
        // GET: API
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;
            ViewData["LdapServersCount"] = e.Ldaps.Count();
            ViewData["WebServicesCount"] = e.WSs.Count();
            ViewData["ExtDatabasesCount"] = e.ExtDBs.Count();
            ViewData["WebDavServersCount"] = e.WebDavServers.Count();
            ViewData["APICount"] = e.APIs.Count();
            ViewData["TCPSocketCount"] = e.TCPListeners.Count();
            return View(e.APIs);
        }

        #region Configuration Methods

        public ActionResult Create()
        {
            return View("~/Views/Nexus/API/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(API model)
        {
            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    API row = e.APIs.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.Definition = model.Definition;
                    
                    e.SaveChanges();
                }
                else
                {
                    e.APIs.Add(model);
                    e.SaveChanges();
                }
                
                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/API/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Nexus/API/Detail.cshtml", e.APIs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Nexus/API/Form.cshtml", e.APIs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            API row = e.APIs.Single(m => m.Id == id);

            e.APIs.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }

        #endregion
        
    }
}