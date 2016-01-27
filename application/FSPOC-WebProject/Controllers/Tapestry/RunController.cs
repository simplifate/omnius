using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        public ActionResult Index(string AppName, int actionRuleId, FormCollection fc, int modelId = -1)
        {
            var core = new Modules.CORE.CORE();
            User user = User.GetLogged();
            Block targetBlock = core.Tapestry.run(user, AppName, actionRuleId, modelId, fc);

            return new RedirectToRouteResult("Mozaic", new RouteValueDictionary(new { Controller = "Show", Action = "Index", appName = AppName, blockId = targetBlock.Id, modelId = modelId }));
        }
    }
}
