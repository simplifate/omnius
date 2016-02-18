using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        public ActionResult Index(string AppName, int actionRuleId, FormCollection fc, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core._form = fc;

            User user = User.GetLogged(core);

            Block targetBlock = core.Tapestry.run(user, AppName, actionRuleId, modelId, fc);

            return new RedirectToRouteResult("Mozaic", new RouteValueDictionary(new { Controller = "Show", Action = "Index", appName = AppName, blockId = targetBlock.Id, modelId = modelId }));
        }

        public ActionResult Generate(string AppName)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = AppName;

            TapestryGeneratorService service = new TapestryGeneratorService();
            service.GenerateTapestry(core);

            return null;
        }
    }
}
