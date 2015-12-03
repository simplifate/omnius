using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.BussinesObjects.Service;

namespace FSPOC_WebProject.Controllers.Nexus
{
    public class WSController : Controller
    {
        // GET: WS
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            return View(e.WSs);
        }

        #region Configuration Methods

        public ActionResult Create()
        {
            return View("~/Views/Nexus/WS/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(WS model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    WS row = e.WSs.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.WSDL_Url = model.WSDL_Url;
                    row.WSDL_File = model.WSDL_File.Length > 0 ? model.WSDL_File : row.WSDL_File;
                    row.Auth_User = model.Auth_User;
                    row.Auth_Password = model.Auth_Password.Length > 0 ? model.Auth_Password : row.Auth_Password;

                    e.SaveChanges();
                }
                else
                {
                    e.WSs.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/WS/Form.cshtml", model);
            }
        }

        #endregion
    }
}