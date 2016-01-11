using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class RightsController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();

            ViewBag.AppNames = e.Applications.Select(a => a.DisplayName);
            ViewBag.Groups = e.Groups.Select(g => g.Name);
            ViewBag.Actions = e.ActionRules.Select(ar => ar.Name);

            return View();
        }
    }
}