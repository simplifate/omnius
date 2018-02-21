using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M = System.Web.Mvc;

namespace FSS.Omnius.FrontEnd.Controllers.Tapestry
{
    public class RunApiController : ApiController
    {
        [Route("api/run/{appName}/{blockIdentify=}")]
        [HttpPost]
        public JToken Run(string appName, string button, [FromBody]JToken body, string blockIdentify, [FromUri]int modelId = -1)
        {
            CORE core = new CORE();
            User currentUser = User.GetLogged(core);
            var fc = new M.FormCollection();
            var inputObject = body as JObject;
            if (inputObject != null)
                foreach (var pair in inputObject)
                    fc.Add(pair.Key, pair.Value.ToString());

            using (DBEntities context = DBEntities.instance)
            {
                // get block
                Block block = null;
                try
                {
                    int blockId = Convert.ToInt32(blockIdentify);
                    block = context.Blocks.FirstOrDefault(b => b.Id == blockId);
                }
                catch (FormatException)
                {
                    block = context.Blocks.FirstOrDefault(b => b.Name == blockIdentify && b.WorkFlow.ApplicationId == core.Application.Id);
                }

                try
                {
                    block = block ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Application.Id && w.InitBlockId != null).InitBlock;
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

        /** 
         * Gathering of deleted blocks and metablocks for Trash dialog purposes (Overview)
         */
        [Route("api/database/apps/{appId}/trashDialog")]
        [HttpGet]
        public List<Object> GetDeletedBlocksList(int appId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
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
            }
            catch (Exception ex)
            {
                var errorMessage = $"Overview: error when loading the blocks history";
                throw ex;
            }
        }
    }
}
