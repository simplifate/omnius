using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class RunController : Controller
    {
        public string Index(int appId, int actionRuleId, int modelId)
        {
            var core = new Modules.CORE.CORE();
            core.Tapestry.run(appId, actionRuleId, modelId);

            return core.Tapestry.GetHtmlOutput();
        }
    }
}
