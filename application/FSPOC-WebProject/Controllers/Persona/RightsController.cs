using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class RightsController : Controller
    {
        // GET: Rights
        public ActionResult ActionIndex()
        {
            DBEntities e = new DBEntities();
            return View(e.ActionRights);
        }

        public ActionResult AppIndex()
        {
            DBEntities e = new DBEntities();

            return View(e.ApplicationRights);
        }
    }
}