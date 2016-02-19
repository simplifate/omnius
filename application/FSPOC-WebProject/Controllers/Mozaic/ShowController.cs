using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Mozaic
{
    [PersonaAuthorize]
    public class ShowController : Controller
    {
        // Run Tapestry & show page
        public string Index(string appName, int blockId, string buttonId, FormCollection fc, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            
            core._form = fc;

            User user = User.GetLogged(core);
            Block targetBlock = core.Tapestry.run(user, appName, blockId, buttonId, modelId, fc);

            return ShowPage(core, appName, targetBlock, modelId);
        }

        // Only show page
        public string Index(string appName, int blockId = -1, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            DBEntities e = core.Entitron.GetStaticTables();
            Block block =
                blockId > 0
                ? e.Blocks.SingleOrDefault(b => b.Id == blockId)
                : e.WorkFlows.SingleOrDefault(wf => wf.ApplicationId == core.Entitron.AppId && wf.Type.Name == "Init").InitBlock;

            return ShowPage(core, appName, block, modelId);
        }

        private string ShowPage(Modules.CORE.CORE core, string appName, Block block, int modelId)
        {
            // init
            core.Entitron.AppName = appName;
            core.User = User.GetLogged(core);
            DBItem model = modelId > 0 ? core.Entitron.GetDynamicItem(block.ModelName, modelId) : null; // can be null

            // preRun
            ActionResultCollection results = new ActionResultCollection();
            results.outputData.Add("__CORE__", core);
            results.outputData.Add("__MODEL__", model);
            block.Run(results);

            // show 
            return block.MozaicPage.ViewContent;
            //return block.MozaicPage.MasterTemplate.Html;
        }
    }
}