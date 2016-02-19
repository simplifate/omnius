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

namespace FSPOC_WebProject.Controllers.Tapestry
{
    [System.Web.Mvc.PersonaAuthorize(Roles = "Admin", Module = "Tapestry")]
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
                        TapestryDesignerBlockCommit blockCommit = requestedBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).First();

                        result = new AjaxTapestryDesignerBlockCommit
                        {
                            Id = blockCommit.Id,
                            Name = blockCommit.Name,
                            AssociatedTableName = blockCommit.AssociatedTableName,
                            PositionX = blockCommit.PositionX,
                            PositionY = blockCommit.PositionY,
                            Timestamp = blockCommit.Timestamp,
                            CommitMessage = blockCommit.CommitMessage
                        };
                        LoadResourceRules(blockCommit, result);
                        LoadWorkflowRules(blockCommit, result);
                    }
                    catch (InvalidOperationException)
                    {
                        result = new AjaxTapestryDesignerBlockCommit
                        {
                            Name = requestedBlock.Name,
                            AssociatedTableName = requestedBlock.AssociatedTableName
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
                        Timestamp = DateTime.Now,
                        CommitMessage = postData.CommitMessage,
                        Name = postData.Name,
                        AssociatedTableName = postData.AssociatedTableName
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
                                ComponentId = ajaxItem.ComponentId
                            };
                            rule.ResourceItems.Add(item);
                            context.SaveChanges();
                            resourceIdMapping.Add(ajaxItem.Id, item.Id);
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
                                    PositionY = ajaxItem.PositionY
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
                                    PositionY = ajaxSymbol.PositionY
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
                    targetBlock.AssociatedTableName = postData.AssociatedTableName;
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
                        AssociatedTableName = blockCommit.AssociatedTableName,
                        PositionX = blockCommit.PositionX,
                        PositionY = blockCommit.PositionY,
                        Timestamp = blockCommit.Timestamp,
                        CommitMessage = blockCommit.CommitMessage
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
                            oldBlock.AssociatedTableName = newBlock.AssociatedTableName;
                            oldBlock.PositionX = newBlock.PositionX;
                            oldBlock.PositionY = newBlock.PositionY;
                            oldBlock.IsInitial = newBlock.IsInitial;
                        }
                    }
                    foreach (var ajaxBlock in postData.Blocks.Where(c => c.IsNew))
                    {
                        int temporaryId = ajaxBlock.Id;
                        var newBlock = new TapestryDesignerBlock
                        {
                            Name = ajaxBlock.Name,
                            AssociatedTableName = ajaxBlock.AssociatedTableName,
                            PositionX = ajaxBlock.PositionX,
                            PositionY = ajaxBlock.PositionY,
                            IsInitial = ajaxBlock.IsInitial
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
                            IsInitial = ajaxMetablock.IsInitial
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
                            AssociatedTableName = sourceBlock.AssociatedTableName,
                            PositionX = sourceBlock.PositionX,
                            PositionY = sourceBlock.PositionY,
                            IsInitial = sourceBlock.IsInitial
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
                            IsInitial = sourceMetablock.IsInitial
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
                    ComponentId = item.ComponentId
                };
                result.ResourceItems.Add(ajaxItem);
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
                    PositionY = item.PositionY
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
                    PositionY = symbol.PositionY
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
                        context.Entry(item).State = EntityState.Deleted;
                    foreach (var connection in rule.Connections)
                        connectionList.Add(connection);
                    foreach (var connection in connectionList)
                        context.Entry(connection).State = EntityState.Deleted;
                    resRuleList.Add(rule);
                }
                foreach (var rule in resRuleList)
                    context.Entry(rule).State = EntityState.Deleted;
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
                            context.Entry(item).State = EntityState.Deleted;
                        foreach (var symbol in swimlane.WorkflowSymbols)
                            symbolList.Add(symbol);
                        foreach (var symbol in symbolList)
                            context.Entry(symbol).State = EntityState.Deleted;
                        context.Entry(swimlane).State = EntityState.Deleted;
                    }
                    foreach (var connection in rule.Connections)
                        connectionList.Add(connection);
                    foreach (var connection in connectionList)
                        context.Entry(connection).State = EntityState.Deleted;
                    wfRuleList.Add(rule);
                }
                foreach (var rule in resRuleList)
                    context.Entry(rule).State = EntityState.Deleted;
                blockCommitList.Add(blockCommit);
            }
            foreach (var blockCommit in blockCommitList)
                context.Entry(blockCommit).State = EntityState.Deleted;
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
