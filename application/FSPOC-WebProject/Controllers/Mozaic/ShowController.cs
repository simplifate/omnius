﻿using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
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
        // GET: Show
        public string Index(string appName, int blockId = -1, int modelId = -1)
        {
            // init
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;
            core.User = User.GetLogged(core);
            DBEntities e = core.Entitron.GetStaticTables();
            Block block =
                blockId > 0
                ? e.Blocks.SingleOrDefault(b => b.Id == blockId)
                : e.WorkFlows.SingleOrDefault(wf => wf.ApplicationId == core.Entitron.AppId && wf.Type.Name == "Init").InitBlock;
            DBItem model = core.Entitron.GetDynamicItem(block.ModelName, modelId); // can be null

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