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

namespace FSPOC_WebProject.Controllers.Tapestry
{
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
                    return context.TapestryDesignerApps
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
                    TapestryDesignerApp app = context.TapestryDesignerApps.Where(a => a.Id == appId).First();
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
                        LoadRules(blockCommit, result);
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
                    Dictionary<int, int> idMapping = new Dictionary<int, int>();
                    var targetBlock = context.TapestryDesignerBlocks.Find(blockId);
                    if(targetBlock == null)
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

                    foreach (AjaxTapestryDesignerRule ajaxRule in postData.Rules)
                    {
                        TapestryDesignerRule rule = new TapestryDesignerRule
                        {
                            Name = ajaxRule.Name,
                            Width = ajaxRule.Width,
                            Height = ajaxRule.Height,
                            PositionX = ajaxRule.PositionX,
                            PositionY = ajaxRule.PositionY
                        };
                        blockCommit.Rules.Add(rule);
                        context.SaveChanges();
                        foreach (AjaxTapestryDesignerItem ajaxItem in ajaxRule.Items)
                        {
                            TapestryDesignerItem item = new TapestryDesignerItem
                            {
                                Label = ajaxItem.Label,
                                TypeClass = ajaxItem.TypeClass,
                                IsDataSource = ajaxItem.IsDataSource,
                                DialogType = ajaxItem.DialogType,
                                PositionX = ajaxItem.PositionX,
                                PositionY = ajaxItem.PositionY
                            };
                            rule.Items.Add(item);
                            context.SaveChanges();
                            idMapping.Add(ajaxItem.Id, item.Id);
                        }
                        foreach (AjaxTapestryDesignerOperator ajaxOperator in ajaxRule.Operators)
                        {
                            TapestryDesignerOperator op = new TapestryDesignerOperator
                            {
                                Type = ajaxOperator.Type,
                                DialogType = ajaxOperator.DialogType,
                                PositionX = ajaxOperator.PositionX,
                                PositionY = ajaxOperator.PositionY
                            };
                            rule.Operators.Add(op);
                            context.SaveChanges();
                            idMapping.Add(ajaxOperator.Id, op.Id);
                        }
                        foreach (AjaxTapestryDesignerConnection ajaxConnection in ajaxRule.Connections)
                        {
                            int source = idMapping[ajaxConnection.Source];
                            int target = idMapping[ajaxConnection.Target];
                            TapestryDesignerConnection connection = new TapestryDesignerConnection
                            {
                                Source = source,
                                Target = target,
                                SourceSlot = ajaxConnection.SourceSlot
                            };
                            rule.Connections.Add(connection);
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
                    LoadRules(blockCommit, result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when loading block data from history (GET api/tapestry/apps/{appId}/blocks/{blockId}/commits/{commitId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
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
                        if(postData.ParentMetablockId != null)
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
                            PositionY = ajaxBlock.PositionY
                        };
                        targetMetablock.Blocks.Add(newBlock);
                        context.SaveChanges();
                        idMapping.BlockIdPairs.Add(new AjaxTapestryDesignerIdPair
                        {
                            TemporaryId = temporaryId,
                            RealId = newBlock.Id
                        });
                    }
                    foreach(var block in blocksToDelete)
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
                        }
                    }
                    foreach (var ajaxMetablock in postData.Metablocks.Where(c => c.IsNew))
                    {
                        int temporaryId = ajaxMetablock.Id;
                        var newMetablock = new TapestryDesignerMetablock
                        {
                            Name = ajaxMetablock.Name,
                            PositionX = ajaxMetablock.PositionX,
                            PositionY = ajaxMetablock.PositionY
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

                    foreach(var sourceBlock in requestedMetablock.Blocks)
                    {
                        blockList.Add(new AjaxTapestryDesignerBlock
                        {
                            Id = sourceBlock.Id,
                            Name = sourceBlock.Name,
                            AssociatedTableName = sourceBlock.AssociatedTableName,
                            PositionX = sourceBlock.PositionX,
                            PositionY = sourceBlock.PositionY
                        });
                    }
                    foreach (var sourceMetablock in requestedMetablock.Metablocks)
                    {
                        metablockList.Add(new AjaxTapestryDesignerMetablock
                        {
                            Id = sourceMetablock.Id,
                            Name = sourceMetablock.Name,
                            PositionX = sourceMetablock.PositionX,
                            PositionY = sourceMetablock.PositionY
                        });
                    }
                    AjaxTapestryDesignerMetablock result = new AjaxTapestryDesignerMetablock
                    {
                        Id = requestedMetablock.Id,
                        Name = requestedMetablock.Name,
                        Blocks = blockList,
                        Metablocks = metablockList
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
        private static void LoadApp(TapestryDesignerApp requestedApp, AjaxTapestryDesignerApp result)
        {
            var ajaxMetablock = new AjaxTapestryDesignerMetablock
            {
                Id = requestedApp.RootMetablock.Id,
                Name = requestedApp.RootMetablock.Name,
                PositionX = requestedApp.RootMetablock.PositionX,
                PositionY = requestedApp.RootMetablock.PositionY
            };
            LoadMetablocks(requestedApp.RootMetablock, ajaxMetablock);
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
        private static void LoadRules(TapestryDesignerBlockCommit requestedBlockCommit, AjaxTapestryDesignerBlockCommit result)
        {
            foreach (TapestryDesignerRule rule in requestedBlockCommit.Rules)
            {
                var ajaxRule = new AjaxTapestryDesignerRule
                {
                    Id = rule.Id,
                    Name = rule.Name,
                    Width = rule.Width,
                    Height = rule.Height,
                    PositionX = rule.PositionX,
                    PositionY = rule.PositionY
                };
                LoadItems(rule, ajaxRule);
                LoadOperators(rule, ajaxRule);
                LoadConnections(rule, ajaxRule);
                result.Rules.Add(ajaxRule);
            }
        }
        private static void LoadItems(TapestryDesignerRule requestedRule, AjaxTapestryDesignerRule result)
        {
            foreach (TapestryDesignerItem item in requestedRule.Items)
            {
                var ajaxItem = new AjaxTapestryDesignerItem
                {
                    Id = item.Id,
                    Label = item.Label,
                    TypeClass = item.TypeClass,
                    IsDataSource = item.IsDataSource,
                    DialogType = item.DialogType,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY
                };
                result.Items.Add(ajaxItem);
            }
        }
        private static void LoadOperators(TapestryDesignerRule requestedRule, AjaxTapestryDesignerRule result)
        {
            foreach (TapestryDesignerOperator op in requestedRule.Operators)
            {
                var ajaxOperator = new AjaxTapestryDesignerOperator
                {
                    Id = op.Id,
                    Type = op.Type,
                    DialogType = op.DialogType,
                    PositionX = op.PositionX,
                    PositionY = op.PositionY
                };
                result.Operators.Add(ajaxOperator);
            }
        }
        private static void LoadConnections(TapestryDesignerRule requestedRule, AjaxTapestryDesignerRule result)
        {
            foreach (TapestryDesignerConnection connection in requestedRule.Connections)
            {
                var ajaxConnection = new AjaxTapestryDesignerConnection
                {
                    Id = connection.Id,
                    Source = connection.Source,
                    SourceSlot = connection.SourceSlot,
                    Target = connection.Target                    
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
                var ruleList = new List<TapestryDesignerRule>();
                foreach (var rule in blockCommit.Rules)
                {
                    var itemList = new List<TapestryDesignerItem>();
                    var connectionList = new List<TapestryDesignerConnection>();
                    foreach (var item in rule.Items)
                        itemList.Add(item);
                    foreach (var item in itemList)
                        context.Entry(item).State = EntityState.Deleted;
                    foreach (var connection in rule.Connections)
                        connectionList.Add(connection);
                    foreach (var connection in connectionList)
                        context.Entry(connection).State = EntityState.Deleted;
                    ruleList.Add(rule);
                }
                foreach(var rule in ruleList)
                    context.Entry(rule).State = EntityState.Deleted;
                blockCommitList.Add(blockCommit);
            }
            foreach (var blockCommit in blockCommitList)
                context.Entry(blockCommit).State = EntityState.Deleted;
            context.Entry(blockToDelete).State = EntityState.Deleted;
        }
        public TapestryApiController() { }
    }
}
