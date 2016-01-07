using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class RightsController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            List<string> actions = new List<string>();
            List<string> apps = new List<string>();
            List<string> groups = new List<string>();
            foreach(ActionRight act in e.ActionRights)
            {
                actions.Add(FSS.Omnius.Modules.Tapestry.Action.All[act.ActionId].Name);
            }
            foreach(AppRight app in e.ApplicationRights)
            {
                apps.Add(app.Application.DisplayName);
            }
            foreach(Group g in e.Groups)
            {
                groups.Add(g.Name);
            }
            ViewBag.AppNames = apps;
            ViewBag.Groups = groups;
            ViewBag.Actions = actions;

            return View();
        }
    }
}