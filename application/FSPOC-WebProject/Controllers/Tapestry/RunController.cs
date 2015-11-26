using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class RunController : Controller
    {
        public string Index(int appId, int actionRuleId, int modelId)
        {
            var core = new Omnius.CORE.CORE();
            core.Tapestry.run(appId, actionRuleId, modelId);

            return core.Tapestry.GetHtmlOutput();
        }
    }
}
