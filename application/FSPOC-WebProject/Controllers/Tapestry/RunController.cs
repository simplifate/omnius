using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class RunController : Controller
    {
        public string Index(int appId, int? actionRuleId, int? modelId, FormCollection fc)
        {
            var core = new Modules.CORE.CORE();
            User user = User.GetLogged();
            core.Tapestry.run(user, appId, actionRuleId, modelId, fc);

            return core.Tapestry.GetHtmlOutput();
        }
    }
}
