using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class ShowController : Controller
    {
        // GET: Show
        public string Index(string appName, int blockId, int modelId = -1)
        {
            // init
            CORE core = new CORE();
            core.Entitron.AppName = appName;
            core.User = User.GetLogged();
            DBEntities e = core.Entitron.GetStaticTables();
            Block block = e.Blocks.SingleOrDefault(b => b.Id == blockId);
            DBItem model = core.Entitron.GetDynamicItem(block.ModelName, modelId); // can be null

            // preRun
            ActionResultCollection results = new ActionResultCollection();
            results.outputData.Add("__CORE__", core);
            results.outputData.Add("__MODEL__", model);
            block.Run(results);

            // show 
            return block.MozaicPage.MasterTemplate.Html;
        }
    }
}