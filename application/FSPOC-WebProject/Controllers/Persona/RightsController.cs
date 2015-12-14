using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
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
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            List<string> actions = new List<string>();
            List<string> apps = new List<string>();
            List<string> groups = new List<string>();
            foreach(ActionRight act in e.ActionRights)
            {
                Dictionary<int,string> names = FSS.Omnius.Modules.Tapestry.Action.AllNames;
                actions.Add(names[act.ActionId]);
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