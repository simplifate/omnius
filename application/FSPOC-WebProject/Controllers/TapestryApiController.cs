using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Entitron.Entity;
using FSS.Omnius.Entitron.Entity.Tapestry;
using Logger;

namespace FSPOC_WebProject.Controllers
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
                    TapestryDesignerBlockCommit blockCommit = context.TapestryDesignerBlocks.Where(b => b.Id == blockId).First()
                        .BlockCommits.OrderByDescending(bc => bc.Timestamp).First();

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
                    TapestryDesignerBlock block = context.TapestryDesignerBlocks.Where(b => b.Id == blockId).First();
                    TapestryDesignerBlockCommit blockCommit = new TapestryDesignerBlockCommit
                    {
                        Timestamp = DateTime.Now,
                        CommitMessage = postData.CommitMessage,
                        Name = postData.Name,
                        AssociatedTableName = postData.AssociatedTableName
                    };
                    block.BlockCommits.Add(blockCommit);

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
                    return context.TapestryDesignerBlocks.Where(b=> b.Id == blockId).First().BlockCommits.OrderByDescending(c => c.Timestamp)
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
            foreach(TapestryDesignerMetaBlock metaBlock in requestedApp.MetaBlocks)
            {
                var ajaxMetaBlock = new AjaxTapestryDesignerMetaBlock
                {
                    Id = metaBlock.Id,
                    Name = metaBlock.Name,
                    PositionX = metaBlock.PositionX,
                    PositionY = metaBlock.PositionY
                };
                LoadMetaBlocks(metaBlock, ajaxMetaBlock);
                result.MetaBlocks.Add(ajaxMetaBlock);
            }
        }
        private static void LoadMetaBlocks(TapestryDesignerMetaBlock requestedMetaBlock, AjaxTapestryDesignerMetaBlock result)
        {
            foreach (TapestryDesignerMetaBlock metaBlock in requestedMetaBlock.MetaBlocks)
            {
                var ajaxMetaBlock = new AjaxTapestryDesignerMetaBlock
                {
                    Id = metaBlock.Id,
                    Name = metaBlock.Name,
                    PositionX = metaBlock.PositionX,
                    PositionY = metaBlock.PositionY
                };
                LoadMetaBlocks(metaBlock, ajaxMetaBlock);
                result.MetaBlocks.Add(ajaxMetaBlock);
            }
            foreach (TapestryDesignerBlock block in requestedMetaBlock.Blocks)
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
        public TapestryApiController() { }
    }
}
