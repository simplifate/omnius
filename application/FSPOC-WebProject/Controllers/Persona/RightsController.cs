using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
    public class RightsController : Controller
    {
        public ActionResult Index()
        {
            DBEntities context = DBEntities.instance;

            ViewBag.ADgroups = context.ADgroups.OrderBy(ad => ad.Name).ToList();
            ViewBag.ADgroups_Users = context.ADgroup_Users.OrderBy(adu => new { adu.User.DisplayName, adu.ADgroup.Name }).ToList();

            return View();
        }

        public ActionResult Refresh()
        {
            CORE core = HttpContext.GetCORE();
            ADgroup.RefreshFromAD(core);

            return RedirectToRoute("Persona", new { controller = "Rights", action = "Index" });
        }
    }
}