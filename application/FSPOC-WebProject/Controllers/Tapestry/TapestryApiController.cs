using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using M = System.Web.Mvc;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Newtonsoft.Json.Linq;

namespace FSPOC_WebProject.Controllers.Tapestry
{
    [M.PersonaAuthorize(NeedsAdmin = true, Module = "Tapestry")]
    public class TapestryApiController : ApiController
    {
        [Route("api/tapestry/apps")]
        [HttpGet]
        public IEnumerable<AjaxTapestryDesignerAppHeader> GetAppList()
        {
            try
            {
                using (var context = new DBEntities())
                {
                    return context.Applications
                        .Select(c => new AjaxTapestryDesignerAppHeader
                        {
                            Id = c.Id,
                            Name = c.Name
                        });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Tapestry Designer: error when loading the app list (GET api/tapestry/apps). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}")]
        [HttpGet]
        public AjaxTapestryDesignerApp GetApp(int appId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    Application app = context.Applications.First(a => a.Id == appId);
                    AjaxTapestryDesignerApp result = new AjaxTapestryDesignerApp
                    {
                        Id = app.Id,
                        Name = app.Name
                    };
                    LoadApp(app, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Tapestry Designer: error when loading app data (GET api/tapestry/apps/{appId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}")]
        [HttpGet]
        public AjaxTapestryDesignerBlockCommit GetBlock(int appId, int blockId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    AjaxTapestryDesignerBlockCommit result;
                    TapestryDesignerBlock requestedBlock = context.TapestryDesignerBlocks.Find(blockId);
                    try
                    {
                        TapestryDesignerBlockCommit blockCommit = requestedBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();

                        if (blockCommit == null)
                        {
                            result = new AjaxTapestryDesignerBlockCommit
                            {
                                Name = requestedBlock.Name,
                                PositionX = requestedBlock.PositionX,
                                PositionY = requestedBlock.PositionY,
                                AssociatedPageIds = new List<int>(),
                                AssociatedTableName = new List<string>(),
                                AssociatedTableIds = new List<int>(),
                                ResourceRules = new List<AjaxTapestryDesignerResourceRule>(),
                                WorkflowRules = new List<AjaxTapestryDesignerWorkflowRule>()
                            };
                        }
                        else
                        {
                            result = new AjaxTapestryDesignerBlockCommit
                            {
                                Id = blockCommit.Id,
                                Name = blockCommit.Name,
                                PositionX = blockCommit.PositionX,
                                PositionY = blockCommit.PositionY,
                                Timestamp = blockCommit.Timestamp,
                                CommitMessage = blockCommit.CommitMessage,
                                AssociatedPageIds = string.IsNullOrEmpty(blockCommit.AssociatedPageIds) ? new List<int>()
                                    : blockCommit.AssociatedPageIds.Split(',').Select(int.Parse).ToList(),
                                AssociatedTableName = string.IsNullOrEmpty(blockCommit.AssociatedTableName) ? new List<string>()
                                    : blockCommit.AssociatedTableName.Split(',').ToList(),
                                AssociatedTableIds = string.IsNullOrEmpty(blockCommit.AssociatedTableIds) ? new List<int>()
                                    : blockCommit.AssociatedTableIds.Split(',').Select(int.Parse).ToList()
                            };
                            LoadResourceRules(blockCommit, result);
                            LoadWorkflowRules(blockCommit, result);
                        }
                    }
                    catch (InvalidOperationException)
                    {

                        result = new AjaxTapestryDesignerBlockCommit
                        {
                            Name = requestedBlock.Name
                        };
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when loading block data (GET api/tapestry/apps/{appId}/blocks/{blockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}")]
        [HttpPost]
        public void SaveBlock(int appId, int blockId, AjaxTapestryDesignerBlockCommit postData)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    Dictionary<int, int> resourceIdMapping = new Dictionary<int, int>();
                    Dictionary<int, int> workflowItemIdMapping = new Dictionary<int, int>();
                    Dictionary<int, int> workflowSymbolIdMapping = new Dictionary<int, int>();
                    var targetBlock = context.TapestryDesignerBlocks.Find(blockId);
                    if (targetBlock == null)
                    {
                        targetBlock = new TapestryDesignerBlock();
                        targetBlock.ParentMetablock = context.TapestryDesignerMetablocks.Find(postData.ParentMetablockId);
                        context.TapestryDesignerBlocks.Add(targetBlock);
                    }
                    TapestryDesignerBlockCommit blockCommit = new TapestryDesignerBlockCommit
                    {
                        Timestamp = DateTime.UtcNow,
                        CommitMessage = postData.CommitMessage,
                        Name = postData.Name,
                        AssociatedPageIds = postData.AssociatedPageIds != null ? string.Join(",", postData.AssociatedPageIds) : "",
                        AssociatedTableName = postData.AssociatedTableName != null ? string.Join(",", postData.AssociatedTableName) : "",
                        AssociatedTableIds = postData.AssociatedTableIds != null ? string.Join(",", postData.AssociatedTableIds) : "",
                    };
                    targetBlock.BlockCommits.Add(blockCommit);

                    foreach (AjaxTapestryDesignerResourceRule ajaxRule in postData.ResourceRules)
                    {
                        TapestryDesignerResourceRule rule = new TapestryDesignerResourceRule
                        {
                            Width = ajaxRule.Width,
                            Height = ajaxRule.Height,
                            PositionX = ajaxRule.PositionX,
                            PositionY = ajaxRule.PositionY
                        };
                        blockCommit.ResourceRules.Add(rule);
                        context.SaveChanges();
                        foreach (AjaxTapestryDesignerResourceItem ajaxItem in ajaxRule.ResourceItems)
                        {
                            TapestryDesignerResourceItem item = new TapestryDesignerResourceItem
                            {
                                Label = ajaxItem.Label,
                                TypeClass = ajaxItem.TypeClass,
                                PositionX = ajaxItem.PositionX,
                                PositionY = ajaxItem.PositionY,
                                PageId = ajaxItem.PageId,
                                ComponentName = ajaxItem.ComponentName,
                                TableName = ajaxItem.TableName,
                                ColumnName = ajaxItem.ColumnName,
                                ColumnFilter = string.Join(",", ajaxItem.ColumnFilter.ToArray())
                            };
                            rule.ResourceItems.Add(item);
                            context.SaveChanges();
                            resourceIdMapping.Add(ajaxItem.Id, item.Id);
                            foreach(AjaxTapestryDesignerConditionSet ajaxConditionSet in ajaxItem.ConditionSets)
                            {
                                TapestryDesignerConditionSet conditionSet = new TapestryDesignerConditionSet
                                {
                                    SetIndex = ajaxConditionSet.SetIndex,
                                    SetRelation = ajaxConditionSet.SetRelation
                                };
                                item.ConditionSets.Add(conditionSet);
                                foreach (AjaxTapestryDesignerCondition ajaxCondition in ajaxConditionSet.Conditions)
                                {
                                    TapestryDesignerCondition condition = new TapestryDesignerCondition
                                    {
                                        Index = ajaxCondition.Index,
                                        Relation = ajaxCondition.Relation,
                                        Variable = ajaxCondition.Variable,
                                        Operator = ajaxCondition.Operator,
                                        Value = ajaxCondition.Value
                                    };
                                    conditionSet.Conditions.Add(condition);
                                }
                            }
                        }
                        foreach (AjaxTapestryDesignerConnection ajaxConnection in ajaxRule.Connections)
                        {
                            int source = resourceIdMapping[ajaxConnection.Source];
                            int target = resourceIdMapping[ajaxConnection.Target];
                            TapestryDesignerConnection connection = new TapestryDesignerConnection
                            {
                                Source = source,
                                SourceType = 0,
                                SourceSlot = 0,
                                Target = target,
                                TargetType = 0,
                                TargetSlot = 0
                            };
                            rule.Connections.Add(connection);
                        }
                    }
                    foreach (AjaxTapestryDesignerWorkflowRule ajaxRule in postData.WorkflowRules)
                    {
                        TapestryDesignerWorkflowRule rule = new TapestryDesignerWorkflowRule
                        {
                            Name = ajaxRule.Name,
                            Width = ajaxRule.Width,
                            Height = ajaxRule.Height,
                            PositionX = ajaxRule.PositionX,
                            PositionY = ajaxRule.PositionY
                        };
                        blockCommit.WorkflowRules.Add(rule);
                        context.SaveChanges();
                        foreach (AjaxTapestryDesignerSwimlane ajaxSwimlane in ajaxRule.Swimlanes)
                        {
                            TapestryDesignerSwimlane swimlane = new TapestryDesignerSwimlane
                            {
                                SwimlaneIndex = ajaxSwimlane.SwimlaneIndex,
                                Height = ajaxSwimlane.Height,
                                Roles = string.Join(",", ajaxSwimlane.Roles.ToArray())
                            };
                            rule.Swimlanes.Add(swimlane);
                            context.SaveChanges();
                            foreach (AjaxTapestryDesignerWorkflowItem ajaxItem in ajaxSwimlane.WorkflowItems)
                            {
                                TapestryDesignerWorkflowItem item = new TapestryDesignerWorkflowItem
                                {
                                    Label = ajaxItem.Label,
                                    TypeClass = ajaxItem.TypeClass,
                                    DialogType = ajaxItem.DialogType,
                                    PositionX = ajaxItem.PositionX,
                                    PositionY = ajaxItem.PositionY,
                                    ActionId = ajaxItem.ActionId,
                                    InputVariables = ajaxItem.InputVariables,
                                    StateId = ajaxItem.StateId,
                                    TargetId = ajaxItem.TargetId,
                                    OutputVariables = ajaxItem.OutputVariables,
                                    ComponentId = ajaxItem.ComponentName
                                };
                                swimlane.WorkflowItems.Add(item);
                                context.SaveChanges();
                                workflowItemIdMapping.Add(ajaxItem.Id, item.Id);
                            }
                            foreach (AjaxTapestryDesignerWorkflowSymbol ajaxSymbol in ajaxSwimlane.WorkflowSymbols)
                            {
                                TapestryDesignerWorkflowSymbol symbol = new TapestryDesignerWorkflowSymbol
                                {
                                    TypeClass = ajaxSymbol.Type,
                                    DialogType = ajaxSymbol.DialogType,
                                    PositionX = ajaxSymbol.PositionX,
                                    PositionY = ajaxSymbol.PositionY,
                                    Condition = ajaxSymbol.Condition
                                };
                                swimlane.WorkflowSymbols.Add(symbol);
                                context.SaveChanges();
                                workflowSymbolIdMapping.Add(ajaxSymbol.Id, symbol.Id);
                            }
                        }
                        foreach (AjaxTapestryDesignerConnection ajaxConnection in ajaxRule.Connections)
                        {
                            int source = ajaxConnection.SourceType == 1 ? workflowSymbolIdMapping[ajaxConnection.Source] : workflowItemIdMapping[ajaxConnection.Source];
                            int target = ajaxConnection.TargetType == 1 ? workflowSymbolIdMapping[ajaxConnection.Target] : workflowItemIdMapping[ajaxConnection.Target];
                            TapestryDesignerConnection connection = new TapestryDesignerConnection
                            {
                                Source = source,
                                SourceType = ajaxConnection.SourceType,
                                SourceSlot = ajaxConnection.SourceSlot,
                                Target = target,
                                TargetType = ajaxConnection.TargetType,
                                TargetSlot = ajaxConnection.TargetSlot
                            };
                            rule.Connections.Add(connection);
                        }
                    }
                    foreach (int portTargetBlockId in postData.PortTargets)
                    {
                        var oldMetablockConnections = context.TapestryDesignerMetablockConnections
                            .Where(c => c.SourceType == 0 && c.SourceId == targetBlock.Id);
                        foreach (var connection in oldMetablockConnections)
                            context.Entry(connection).State = EntityState.Deleted;
                        TapestryDesignerMetablock nearbyMetablock = new TapestryDesignerMetablock();
                        var portTargetBlock = context.TapestryDesignerBlocks.Find(portTargetBlockId);
                        if (portTargetBlock.ParentMetablock.Id == targetBlock.ParentMetablock.Id)
                            targetBlock.ParentMetablock.Connections.Add(new TapestryDesignerMetablockConnection
                            {
                                SourceType = 0,
                                TargetType = 0,
                                SourceId = blockId,
                                TargetId = portTargetBlockId
                            });
                        else if (GetNearbyAncestor(targetBlock.ParentMetablock, portTargetBlock.ParentMetablock, out nearbyMetablock, context))
                        {
                            targetBlock.ParentMetablock.Connections.Add(new TapestryDesignerMetablockConnection
                            {
                                SourceType = 0,
                                TargetType = 1,
                                SourceId = blockId,
                                TargetId = nearbyMetablock.Id
                            });
                        }
                    }
                    targetBlock.Name = postData.Name;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when saving block data (POST api/tapestry/apps/{appId}/blocks/{blockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/commits")]
        [HttpGet]
        public IEnumerable<AjaxTapestryDesignerBlockCommitHeader> GetBlockHistory(int appId, int blockId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    return context.TapestryDesignerBlocks.Find(blockId).BlockCommits.OrderByDescending(c => c.Timestamp)
                        .Select(c => new AjaxTapestryDesignerBlockCommitHeader
                        {
                            Id = c.Id,
                            CommitMessage = c.CommitMessage,
                            Timestamp = c.Timestamp
                        });
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when loading block history (GET api/tapestry/apps/{appId}/blocks/{blockId}/commits). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/commits/{commitId}")]
        [HttpGet]
        public AjaxTapestryDesignerBlockCommit GetSpecificBlockCommit(int appId, int blockId, int commitId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    TapestryDesignerBlockCommit blockCommit = context.TapestryDesignerBlocks.Where(b => b.Id == blockId).First()
                        .BlockCommits.Where(c => c.Id == commitId).First();

                    AjaxTapestryDesignerBlockCommit result = new AjaxTapestryDesignerBlockCommit
                    {
                        Id = blockCommit.Id,
                        Name = blockCommit.Name,
                        PositionX = blockCommit.PositionX,
                        PositionY = blockCommit.PositionY,
                        Timestamp = blockCommit.Timestamp,
                        CommitMessage = blockCommit.CommitMessage,
                        AssociatedPageIds = string.IsNullOrEmpty(blockCommit.AssociatedPageIds) ? new List<int>()
                                : blockCommit.AssociatedPageIds.Split(',').Select(int.Parse).ToList(),
                        AssociatedTableName = string.IsNullOrEmpty(blockCommit.AssociatedTableName) ? new List<string>()
                                : blockCommit.AssociatedTableName.Split(',').ToList(),
                        AssociatedTableIds = string.IsNullOrEmpty(blockCommit.AssociatedTableIds) ? new List<int>()
                                : blockCommit.AssociatedTableIds.Split(',').Select(int.Parse).ToList()
                    };
                    LoadResourceRules(blockCommit, result);
                    LoadWorkflowRules(blockCommit, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when loading block data from history (GET api/tapestry/apps/{appId}/blocks/{blockId}/commits/{commitId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/blocks")]
        [HttpGet]
        public AjaxTapestryDesignerBlockList GetBlockList(int appId)
        {
            using (var context = new DBEntities())
            {
                var result = new AjaxTapestryDesignerBlockList();
                var rootMetablock = context.Applications.Find(appId).TapestryDesignerRootMetablock;
                CollectBlocksToList(rootMetablock, result, context);
                return result;
            }
        }
        [Route("api/tapestry/apps/{appId}/metablocks/{metablockId}")]
        [HttpPost]
        public AjaxTapestryDesignerIdMapping SaveMetablock(int appId, int metablockId, AjaxTapestryDesignerMetablock postData)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    AjaxTapestryDesignerIdMapping idMapping = new AjaxTapestryDesignerIdMapping();
                    var targetMetablock = context.TapestryDesignerMetablocks.Find(metablockId);
                    if (targetMetablock == null)
                    {
                        targetMetablock = new TapestryDesignerMetablock();
                        if (postData.ParentMetablockId != null)
                            targetMetablock.ParentMetablock = context.TapestryDesignerMetablocks.Find(postData.ParentMetablockId);

                        context.TapestryDesignerMetablocks.Add(targetMetablock);
                    }
                    targetMetablock.Name = postData.Name;
                    var blocksToDelete = new List<TapestryDesignerBlock>();
                    foreach (var oldBlock in targetMetablock.Blocks)
                    {
                        var match = postData.Blocks.Where(c => !c.IsNew && c.Id == oldBlock.Id);
                        if (match.Count() == 0)
                        {
                            blocksToDelete.Add(oldBlock);
                        }
                        else
                        {
                            var newBlock = match.First();
                            oldBlock.Name = newBlock.Name;
                            oldBlock.PositionX = newBlock.PositionX;
                            oldBlock.PositionY = newBlock.PositionY;
                            oldBlock.IsInitial = newBlock.IsInitial;
                            oldBlock.IsInMenu = newBlock.IsInMenu;
                        }
                    }
                    foreach (var ajaxBlock in postData.Blocks.Where(c => c.IsNew))
                    {
                        int temporaryId = ajaxBlock.Id;
                        var newBlock = new TapestryDesignerBlock
                        {
                            Name = ajaxBlock.Name,
                            PositionX = ajaxBlock.PositionX,
                            PositionY = ajaxBlock.PositionY,
                            IsInitial = ajaxBlock.IsInitial,
                            IsInMenu = ajaxBlock.IsInMenu
                        };
                        targetMetablock.Blocks.Add(newBlock);
                        context.SaveChanges();
                        idMapping.BlockIdPairs.Add(new AjaxTapestryDesignerIdPair
                        {
                            TemporaryId = temporaryId,
                            RealId = newBlock.Id
                        });
                    }
                    foreach (var block in blocksToDelete)
                    {
                        DeleteBlock(block, context);
                    }
                    var metablocksToDelete = new List<TapestryDesignerMetablock>();
                    foreach (var oldMetablock in targetMetablock.Metablocks)
                    {
                        var match = postData.Metablocks.Where(c => !c.IsNew && c.Id == oldMetablock.Id);
                        if (match.Count() == 0)
                        {
                            metablocksToDelete.Add(oldMetablock);
                        }
                        else
                        {
                            var newMetablock = match.First();
                            oldMetablock.Name = newMetablock.Name;
                            oldMetablock.PositionX = newMetablock.PositionX;
                            oldMetablock.PositionY = newMetablock.PositionY;
                            oldMetablock.IsInitial = newMetablock.IsInitial;
                            oldMetablock.IsInMenu = newMetablock.IsInMenu;
                        }
                    }
                    foreach (var ajaxMetablock in postData.Metablocks.Where(c => c.IsNew))
                    {
                        int temporaryId = ajaxMetablock.Id;
                        var newMetablock = new TapestryDesignerMetablock
                        {
                            Name = ajaxMetablock.Name,
                            PositionX = ajaxMetablock.PositionX,
                            PositionY = ajaxMetablock.PositionY,
                            IsInitial = ajaxMetablock.IsInitial,
                            IsInMenu = ajaxMetablock.IsInMenu
                        };
                        targetMetablock.Metablocks.Add(newMetablock);
                        context.SaveChanges();
                        idMapping.MetablockIdPairs.Add(new AjaxTapestryDesignerIdPair
                        {
                            TemporaryId = temporaryId,
                            RealId = newMetablock.Id
                        });
                    }
                    foreach (var metablock in metablocksToDelete)
                    {
                        DeleteMetablock(metablock, context);
                    }
                    context.SaveChanges();
                    return idMapping;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Overview: error saving metablock (POST api/tapestry/apps/{appId}/metablocks/{metablockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/tapestry/apps/{appId}/metablocks/{metablockId}")]
        [HttpGet]
        public AjaxTapestryDesignerMetablock LoadMetablock(int appId, int metablockId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    var requestedMetablock = context.TapestryDesignerMetablocks.Find(metablockId);
                    var blockList = new List<AjaxTapestryDesignerBlock>();
                    var metablockList = new List<AjaxTapestryDesignerMetablock>();
                    var connectionList = new List<AjaxTapestryDesignerMetablockConnection>();

                    foreach (var sourceBlock in requestedMetablock.Blocks)
                    {
                        blockList.Add(new AjaxTapestryDesignerBlock
                        {
                            Id = sourceBlock.Id,
                            Name = sourceBlock.Name,
                            PositionX = sourceBlock.PositionX,
                            PositionY = sourceBlock.PositionY,
                            IsInitial = sourceBlock.IsInitial,
                            IsInMenu = sourceBlock.IsInMenu
                        });
                    }
                    foreach (var sourceMetablock in requestedMetablock.Metablocks)
                    {
                        metablockList.Add(new AjaxTapestryDesignerMetablock
                        {
                            Id = sourceMetablock.Id,
                            Name = sourceMetablock.Name,
                            PositionX = sourceMetablock.PositionX,
                            PositionY = sourceMetablock.PositionY,
                            IsInitial = sourceMetablock.IsInitial,
                            IsInMenu = sourceMetablock.IsInMenu
                        });
                    }
                    foreach (var sourceMetablockConnection in requestedMetablock.Connections)
                    {
                        connectionList.Add(new AjaxTapestryDesignerMetablockConnection
                        {
                            Id = sourceMetablockConnection.Id,
                            SourceType = sourceMetablockConnection.SourceType,
                            TargetType = sourceMetablockConnection.TargetType,
                            SourceId = sourceMetablockConnection.SourceId,
                            TargetId = sourceMetablockConnection.TargetId
                        });
                    }
                    AjaxTapestryDesignerMetablock result = new AjaxTapestryDesignerMetablock
                    {
                        Id = requestedMetablock.Id,
                        Name = requestedMetablock.Name,
                        Blocks = blockList,
                        Metablocks = metablockList,
                        Connections = connectionList
                    };
                    return result;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Overview: error when loading metablock data (GET api/tapestry/apps/{appId}/metablocks/{metablockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/tapestry/actions")]
        [HttpGet]
        public AjaxTransferTapestryActionList GetActionList()
        {
            AjaxTransferTapestryActionList result = new AjaxTransferTapestryActionList();

            foreach(KeyValuePair<int, FSS.Omnius.Modules.Tapestry.Action> action in FSS.Omnius.Modules.Tapestry.Action.All.OrderBy(a => a.Value.Name)) {
                result.Items.Add(new AjaxTransferTapestryActionItem()
                {
                    Id = action.Value.Id,
                    ReverseActionId = action.Value.ReverseActionId,
                    InputVars = action.Value.InputVar,
                    OutputVars = action.Value.OutputVar,
                    Name = action.Value.Name
                });
            };
            return result;
        }

        [Route("api/tapestry/saveMenuOrder")]
        [HttpPost]
        public HttpResponseMessage SaveMenuOrder(AjaxTransferMenuOrder data)
        {
            try
            {
                using (DBEntities context = new DBEntities())
                {
                    foreach (KeyValuePair<int, int> row in data.Metablocks)
                    {
                        TapestryDesignerMetablock metablock = context.TapestryDesignerMetablocks.Where(m => m.Id == row.Key).First();
                        metablock.MenuOrder = row.Value;
                    }
                    foreach (KeyValuePair<int, int> row in data.Blocks)
                    {
                        TapestryDesignerBlock block = context.TapestryDesignerBlocks.Where(b => b.Id == row.Key).First();
                        block.MenuOrder = row.Value;
                    }
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Overview: error when saving menu order (POST api/tapestry/saveMenuOrder). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/run/{appName}/{blockId}")]
        public JToken Run(string appName, string button, M.FormCollection fc, int blockId = -1, int modelId = -1)
        {
            CORE core = new CORE();
            User currentUser = User.GetLogged(core);

            using (DBEntities context = new DBEntities())
            {
                Block block = context.Blocks.SingleOrDefault(b => b.Id == blockId) ?? context.WorkFlows.FirstOrDefault(w => w.Application.Name == appName && w.Type.Name == "Init").InitBlock;
                var result = core.Tapestry.jsonRun(currentUser, appName, block, button, modelId, fc);

                return result;
            }
        }

        private static HttpResponseException GetHttpInternalServerErrorResponseException(string errorMessage)
        {
            Log.Error(errorMessage);
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(errorMessage),
                ReasonPhrase = "Critical Exception"
            });
        }
        private static void LoadApp(Application requestedApp, AjaxTapestryDesignerApp result)
        {
            var ajaxMetablock = new AjaxTapestryDesignerMetablock
            {
                Id = requestedApp.TapestryDesignerRootMetablock.Id,
                Name = requestedApp.TapestryDesignerRootMetablock.Name,
                PositionX = requestedApp.TapestryDesignerRootMetablock.PositionX,
                PositionY = requestedApp.TapestryDesignerRootMetablock.PositionY
            };
            LoadMetablocks(requestedApp.TapestryDesignerRootMetablock, ajaxMetablock);
            result.RootMetablock = ajaxMetablock;
        }
        private static void LoadMetablocks(TapestryDesignerMetablock requestedMetablock, AjaxTapestryDesignerMetablock result)
        {
            foreach (TapestryDesignerMetablock metablock in requestedMetablock.Metablocks)
            {
                var ajaxMetablock = new AjaxTapestryDesignerMetablock
                {
                    Id = metablock.Id,
                    Name = metablock.Name,
                    PositionX = metablock.PositionX,
                    PositionY = metablock.PositionY
                };
                LoadMetablocks(metablock, ajaxMetablock);
                result.Metablocks.Add(ajaxMetablock);
            }
            foreach (TapestryDesignerBlock block in requestedMetablock.Blocks)
            {
                var ajaxBlock = new AjaxTapestryDesignerBlock
                {
                    Id = block.Id,
                    Name = block.Name,
                    PositionX = block.PositionX,
                    PositionY = block.PositionY
                };
                result.Blocks.Add(ajaxBlock);
            }
        }
        private static void LoadResourceRules(TapestryDesignerBlockCommit requestedBlockCommit, AjaxTapestryDesignerBlockCommit result)
        {
            foreach (TapestryDesignerResourceRule rule in requestedBlockCommit.ResourceRules)
            {
                var ajaxRule = new AjaxTapestryDesignerResourceRule
                {
                    Id = rule.Id,
                    Width = rule.Width,
                    Height = rule.Height,
                    PositionX = rule.PositionX,
                    PositionY = rule.PositionY
                };
                LoadResourceItems(rule, ajaxRule);
                LoadConnections(rule, ajaxRule);
                result.ResourceRules.Add(ajaxRule);
            }
        }
        private static void LoadResourceItems(TapestryDesignerResourceRule requestedRule, AjaxTapestryDesignerResourceRule result)
        {
            foreach (TapestryDesignerResourceItem item in requestedRule.ResourceItems)
            {
                var ajaxItem = new AjaxTapestryDesignerResourceItem
                {
                    Id = item.Id,
                    Label = item.Label,
                    TypeClass = item.TypeClass,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    PageId = item.PageId,
                    ComponentName = item.ComponentName,
                    TableName = item.TableName,
                    ColumnName = item.ColumnName,
                    ColumnFilter = string.IsNullOrEmpty(item.ColumnFilter) ? new List<int>() : item.ColumnFilter.Split(',').Select(int.Parse).ToList()
                };
                LoadConditionSets(item, ajaxItem);
                result.ResourceItems.Add(ajaxItem);
            }
        }
        private static void LoadConditionSets(TapestryDesignerResourceItem requestedItem, AjaxTapestryDesignerResourceItem result)
        {
            foreach (TapestryDesignerConditionSet conditionSet in requestedItem.ConditionSets)
            {
                var ajaxConditionSet = new AjaxTapestryDesignerConditionSet
                {
                    Id = conditionSet.Id,
                    SetIndex = conditionSet.SetIndex,
                    SetRelation = conditionSet.SetRelation
                };
                LoadConditions(conditionSet, ajaxConditionSet);
                result.ConditionSets.Add(ajaxConditionSet);
            }
        }
        private static void LoadConditions(TapestryDesignerConditionSet requestedConditionSet, AjaxTapestryDesignerConditionSet result)
        {
            foreach (TapestryDesignerCondition condition in requestedConditionSet.Conditions)
            {
                var ajaxCondition = new AjaxTapestryDesignerCondition
                {
                    Id = condition.Id,
                    Index = condition.Index,
                    Relation = condition.Relation,
                    Variable = condition.Variable,
                    Operator = condition.Operator,
                    Value = condition.Value
                };
                result.Conditions.Add(ajaxCondition);
            }
        }
        private static void LoadWorkflowRules(TapestryDesignerBlockCommit requestedBlockCommit, AjaxTapestryDesignerBlockCommit result)
        {
            foreach (TapestryDesignerWorkflowRule rule in requestedBlockCommit.WorkflowRules)
            {
                var ajaxRule = new AjaxTapestryDesignerWorkflowRule
                {
                    Id = rule.Id,
                    Name = rule.Name,
                    Width = rule.Width,
                    Height = rule.Height,
                    PositionX = rule.PositionX,
                    PositionY = rule.PositionY
                };
                LoadSwimlanes(rule, ajaxRule);
                LoadConnections(rule, ajaxRule);
                result.WorkflowRules.Add(ajaxRule);
            }
        }
        private static void LoadSwimlanes(TapestryDesignerWorkflowRule requestedRule, AjaxTapestryDesignerWorkflowRule result)
        {
            foreach (TapestryDesignerSwimlane swimlane in requestedRule.Swimlanes)
            {
                var ajaxSwimlane = new AjaxTapestryDesignerSwimlane
                {
                    Id = swimlane.Id,
                    SwimlaneIndex = swimlane.SwimlaneIndex,
                    Height = swimlane.Height,
                    Roles = string.IsNullOrEmpty(swimlane.Roles) ? new List<string>() : swimlane.Roles.Split(',').ToList()
                };
                LoadWorkflowItems(swimlane, ajaxSwimlane);
                LoadWorkflowSymbols(swimlane, ajaxSwimlane);
                result.Swimlanes.Add(ajaxSwimlane);
            }
        }
        private static void LoadWorkflowItems(TapestryDesignerSwimlane requestedSwimlane, AjaxTapestryDesignerSwimlane result)
        {
            foreach (TapestryDesignerWorkflowItem item in requestedSwimlane.WorkflowItems)
            {
                var ajaxItem = new AjaxTapestryDesignerWorkflowItem
                {
                    Id = item.Id,
                    Label = item.Label,
                    TypeClass = item.TypeClass,
                    DialogType = item.DialogType,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    ActionId = item.ActionId,
                    InputVariables = item.InputVariables,
                    OutputVariables = item.OutputVariables,
                    StateId = item.StateId,
                    ComponentName = item.ComponentId,
                    TargetId = item.TargetId
                };
                result.WorkflowItems.Add(ajaxItem);
            }
        }
        private static void LoadWorkflowSymbols(TapestryDesignerSwimlane requestedSwimlane, AjaxTapestryDesignerSwimlane result)
        {
            foreach (TapestryDesignerWorkflowSymbol symbol in requestedSwimlane.WorkflowSymbols)
            {
                var ajaxSymbol = new AjaxTapestryDesignerWorkflowSymbol
                {
                    Id = symbol.Id,
                    Type = symbol.TypeClass,
                    DialogType = symbol.DialogType,
                    PositionX = symbol.PositionX,
                    PositionY = symbol.PositionY,
                    Condition = symbol.Condition
                };
                result.WorkflowSymbols.Add(ajaxSymbol);
            }
        }
        private static void LoadConnections(TapestryDesignerResourceRule requestedRule, AjaxTapestryDesignerResourceRule result)
        {
            foreach (TapestryDesignerConnection connection in requestedRule.Connections)
            {
                var ajaxConnection = new AjaxTapestryDesignerConnection
                {
                    Id = connection.Id,
                    Source = connection.Source,
                    SourceType = connection.SourceType,
                    SourceSlot = connection.SourceSlot,
                    Target = connection.Target,
                    TargetType = connection.TargetType,
                    TargetSlot = connection.TargetSlot
                };
                result.Connections.Add(ajaxConnection);
            }
        }
        private static void LoadConnections(TapestryDesignerWorkflowRule requestedRule, AjaxTapestryDesignerWorkflowRule result)
        {
            foreach (TapestryDesignerConnection connection in requestedRule.Connections)
            {
                var ajaxConnection = new AjaxTapestryDesignerConnection
                {
                    Id = connection.Id,
                    Source = connection.Source,
                    SourceType = connection.SourceType,
                    SourceSlot = connection.SourceSlot,
                    Target = connection.Target,
                    TargetType = connection.TargetType,
                    TargetSlot = connection.TargetSlot
                };
                result.Connections.Add(ajaxConnection);
            }
        }
        private static void DeleteMetablock(TapestryDesignerMetablock metablockToDelete, DBEntities context)
        {
            var blockList = new List<TapestryDesignerBlock>();
            var metablockList = new List<TapestryDesignerMetablock>();
            foreach (var metablock in metablockToDelete.Metablocks)
                metablockList.Add(metablock);
            foreach (var metablock in metablockList)
                DeleteMetablock(metablock, context);
            foreach (var block in metablockToDelete.Blocks)
                blockList.Add(block);
            foreach (var block in blockList)
                DeleteBlock(block, context);
            context.Entry(metablockToDelete).State = EntityState.Deleted;
        }
        private static void DeleteBlock(TapestryDesignerBlock blockToDelete, DBEntities context)
        {
            var blockCommitList = new List<TapestryDesignerBlockCommit>();
            foreach (var blockCommit in blockToDelete.BlockCommits)
            {
                var resRuleList = new List<TapestryDesignerResourceRule>();
                foreach (var rule in blockCommit.ResourceRules)
                {
                    var itemList = new List<TapestryDesignerResourceItem>();
                    var connectionList = new List<TapestryDesignerConnection>();
                    foreach (var item in rule.ResourceItems)
                        itemList.Add(item);
                    foreach (var item in itemList)
                        rule.ResourceItems.Remove(item);
                    foreach (var connection in rule.Connections)
                        connectionList.Add(connection);
                    foreach (var connection in connectionList)
                        rule.Connections.Remove(connection);
                    resRuleList.Add(rule);
                }
                foreach (var rule in resRuleList)
                    blockCommit.ResourceRules.Remove(rule);
                var wfRuleList = new List<TapestryDesignerWorkflowRule>();
                foreach (var rule in blockCommit.WorkflowRules)
                {
                    var swimlaneList = new List<TapestryDesignerSwimlane>();
                    var connectionList = new List<TapestryDesignerConnection>();
                    foreach (var swimlane in rule.Swimlanes)
                        swimlaneList.Add(swimlane);
                    foreach (var swimlane in swimlaneList)
                    {
                        var itemList = new List<TapestryDesignerWorkflowItem>();
                        var symbolList = new List<TapestryDesignerWorkflowSymbol>();
                        foreach (var item in swimlane.WorkflowItems)
                            itemList.Add(item);
                        foreach (var item in itemList)
                            swimlane.WorkflowItems.Remove(item);
                        foreach (var symbol in swimlane.WorkflowSymbols)
                            symbolList.Add(symbol);
                        foreach (var symbol in symbolList)
                            swimlane.WorkflowSymbols.Remove(symbol);
                        rule.Swimlanes.Remove(swimlane);
                    }
                    foreach (var connection in rule.Connections)
                        connectionList.Add(connection);
                    foreach (var connection in connectionList)
                        rule.Connections.Remove(connection);
                    wfRuleList.Add(rule);
                }
                foreach (var rule in wfRuleList)
                    blockCommit.WorkflowRules.Remove(rule);
                blockCommitList.Add(blockCommit);
            }
            foreach (var blockCommit in blockCommitList)
                blockToDelete.BlockCommits.Remove(blockCommit);
            context.Entry(blockToDelete).State = EntityState.Deleted;
        }
        private void CollectBlocksToList(TapestryDesignerMetablock rootMetablock,
            AjaxTapestryDesignerBlockList list, DBEntities context)
        {
            foreach (TapestryDesignerBlock block in rootMetablock.Blocks)
                list.ListItems.Add(new AjaxTapestryDesignerBlockListItem
                {
                    Id = block.Id,
                    Name = block.Name
                });
            foreach (TapestryDesignerMetablock metablock in rootMetablock.Metablocks)
                CollectBlocksToList(metablock, list, context);
        }
        private bool GetNearbyAncestor(TapestryDesignerMetablock environment, TapestryDesignerMetablock currentMetablock,
            out TapestryDesignerMetablock nearbyMetablock, DBEntities context)
        {
            var parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock")
                .Where(c => c.Id == currentMetablock.Id).First().ParentMetablock;
            if (parentMetablock == null)
            {
                nearbyMetablock = null;
                return false;
            }
            else if (parentMetablock.Id == environment.Id)
            {
                nearbyMetablock = currentMetablock;
                return true;
            }
            else
            {
                return GetNearbyAncestor(environment, parentMetablock, out nearbyMetablock, context);
            }
        }
        public TapestryApiController() { }
    }
}
