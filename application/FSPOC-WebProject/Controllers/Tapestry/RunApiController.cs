using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M = System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Tapestry
{
    public class RunApiController : ApiController
    {
        [Route("api/run/{appName}/{blockIdentify=}")]
        [HttpPost]
        public JToken Run(string appName, string button, M.FormCollection fc, string blockIdentify, int modelId = -1)
        {
            CORE core = new CORE();
            core.Entitron.Application = core.Entitron.GetStaticTables().Applications.SingleOrDefault(a => a.Name == appName && a.IsEnabled && a.IsPublished && !a.IsSystem);
            User currentUser = User.GetLogged(core);

            using (DBEntities context = new DBEntities())
            {
                // get block
                Block block = null;
                try
                {
                    int blockId = Convert.ToInt32(blockIdentify);
                    block = context.Blocks.SingleOrDefault(b => b.Id == blockId);
                }
                catch (FormatException)
                {
                    block = context.Blocks.SingleOrDefault(b => b.Name == blockIdentify);
                }

                try
                {
                    block = block ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Entitron.AppId && w.InitBlockId != null).InitBlock;
                }
                catch (NullReferenceException)
                {
                    return null;
                }

                // RUN
                var result = core.Tapestry.jsonRun(currentUser, block, button, modelId, fc);

                return result;
            }
        }
    }
}
