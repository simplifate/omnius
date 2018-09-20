using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FSS.Omnius.FrontEnd.Controllers.Persona
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
    public class RightsController : Controller
    {
        public ActionResult Index()
        {
            DBEntities context = COREobject.i.Context;

            ViewBag.ADgroups = context.ADgroups.OrderBy(ad => ad.Name).ToList();
            ViewBag.APPS = context.Applications.OrderBy(app => app.Id).ToList();
            ViewBag.ADgroups_Users = context.ADgroup_Users.OrderBy(adu => new { adu.User.DisplayName, adu.ADgroup.Name }).ToList();

            return View();
        }
    }
}