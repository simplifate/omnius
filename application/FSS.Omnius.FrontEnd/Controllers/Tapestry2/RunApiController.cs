using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.FrontEnd.Controllers.Tapestry2
{
    public class RunApiController : ApiController
    {
        [Route("T2/api/run/{appName}/{blockName=}")]
        [HttpPost]
        public string Run(string appName, string button, [FromBody]JToken body, string blockName, [FromUri]int modelId = -1)
        {
            COREobject core = COREobject.i;
            DBEntities context = core.Context;

            core.ModelId = modelId;
            var inputObject = body as JObject;
            if (inputObject != null)
                foreach (var pair in inputObject)
                    core.Data.Add(pair.Key, pair.Value.ToString());

            // RUN
            var tapestry = new Modules.Tapestry2.Tapestry(core);
            var jsonResult = tapestry.jsonRun(blockName, button);

            return jsonResult.ToString();
        }

        /** 
         * Gathering of deleted blocks and metablocks for Trash dialog purposes (Overview)
         */
        [Route("T2/api/database/apps/{appId}/trashDialog")]
        [HttpGet]
        public List<Object> GetDeletedBlocksList(int appId)
        {
            try
            {
                var context = COREobject.i.Context;
                var result = new List<Object>();

                var blocksList = new List<AjaxTapestryDesignerBlock>();
                var metablocksList = new List<AjaxTapestryDesignerMetablock>();
                var requestedApp = context.Applications.Find(appId);

                foreach (var block in context.TapestryDesignerBlocks.Where(b => b.IsDeleted && b.ParentMetablock.ParentAppId == appId))
                {
                    blocksList.Add(new AjaxTapestryDesignerBlock
                    {
                        Id = block.Id,
                        Name = block.Name,
                        PositionX = block.PositionX,
                        PositionY = block.PositionY,
                        MenuOrder = block.MenuOrder,
                        IsInitial = block.IsInitial,
                        IsInMenu = block.IsInMenu
                    });
                }

                foreach (var metablock in requestedApp.TapestryDesignerMetablocks.Where(b => b.IsDeleted))
                {
                    metablocksList.Add(new AjaxTapestryDesignerMetablock
                    {
                        Id = metablock.Id,
                        Name = metablock.Name,
                        PositionX = metablock.PositionX,
                        PositionY = metablock.PositionY,
                        ParentAppId = metablock.ParentAppId,
                        IsInitial = metablock.IsInitial,
                        IsInMenu = metablock.IsInMenu,
                    });
                }

                result.Add(blocksList);
                result.Add(metablocksList);

                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Overview: error when loading the blocks history";
                throw ex;
            }
        }
    }
}
