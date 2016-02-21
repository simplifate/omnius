using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using TapestryModule = FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        // Run Tapestry & show page
        [HttpPost]
        public ActionResult Index(string appName, int blockId, string buttonId, FormCollection fc, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;

            core._form = fc;

            User user = User.GetLogged(core);
            Block targetBlock = core.Tapestry.run(user, appName, blockId, buttonId, modelId, fc);

            return ShowPage(core, appName, targetBlock, modelId);
        }

        // Only show page
        [HttpGet]
        public ActionResult Index(string appName, int blockId = -1, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;
            DBEntities e = core.Entitron.GetStaticTables();
            Block block = blockId > 0
                ? e.Blocks.SingleOrDefault(b => b.Id == blockId)
                : e.WorkFlows.SingleOrDefault(wf => wf.ApplicationId == core.Entitron.AppId && wf.Type.Name == "Init").InitBlock;

            return ShowPage(core, appName, block, modelId);
        }

        private ActionResult ShowPage(Modules.CORE.CORE core, string appName, Block block, int modelId)
        {
            // init
            core.User = User.GetLogged(core);
            DBItem model = modelId > 0 ? core.Entitron.GetDynamicItem(block.ModelName, modelId) : null; // can be null

            // preRun
            TapestryModule.ActionResultCollection results = new TapestryModule.ActionResultCollection();
            results.outputData.Add("__CORE__", core);
            results.outputData.Add("__MODEL__", model);
            block.Run(results);

            //
            foreach(var pair in results.outputData)
            {
                ViewData.Add(pair);
            }

            // show
            return View(block.MozaicPage.ViewPath);
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
