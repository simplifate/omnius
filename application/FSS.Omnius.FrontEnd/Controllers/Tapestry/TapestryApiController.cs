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
using System.Web;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.FrontEnd.Controllers.Tapestry
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
                using (var context = DBEntities.instance)
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
                using (var context = DBEntities.instance)
                {
                    Application app = context.Applications.Find(appId);
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
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/getLastCommit")]
        [HttpGet]
        public int? GetLastCommitId(int appId, int blockId)
        {
            using (var context = DBEntities.instance)
            {
                TapestryDesignerBlock requestedBlock = context.TapestryDesignerBlocks.Find(blockId);
                    TapestryDesignerBlockCommit blockCommit = requestedBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                    if(blockCommit!=null)
                        return blockCommit.Id;
                return null;
             }
        }

        [Route("api/tapestry/apps/{appId}/blocks/{blockId}")]
        [HttpGet]
        public AjaxTapestryDesignerBlockCommit GetBlock(int appId, int blockId)
        {
            try
            {
                using (var context = DBEntities.instance)
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
                                AssociatedBootstrapPageIds = new List<int>(),
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
                                ModelTableName = blockCommit.ModelTableName,
                                LockedForUserId = requestedBlock.LockedForUserId,
                                LockedForUserName = requestedBlock.LockedForUserId != null ? context.Users.FirstOrDefault(u => u.Id == requestedBlock.LockedForUserId).DisplayName : "",
                                AssociatedPageIds = string.IsNullOrEmpty(blockCommit.AssociatedPageIds) ? new List<int>()
                                    : blockCommit.AssociatedPageIds.Split(',').Select(int.Parse).ToList(),
                                AssociatedBootstrapPageIds = string.IsNullOrEmpty(blockCommit.AssociatedBootstrapPageIds) ? new List<int>()
                                    : blockCommit.AssociatedBootstrapPageIds.Split(',').Select(int.Parse).ToList(),
                                AssociatedTableName = string.IsNullOrEmpty(blockCommit.AssociatedTableName) ? new List<string>()
                                    : blockCommit.AssociatedTableName.Split(',').ToList(),
                                AssociatedTableIds = string.IsNullOrEmpty(blockCommit.AssociatedTableIds) ? new List<int>()
                                    : blockCommit.AssociatedTableIds.Split(',').Select(int.Parse).ToList(),
                                RoleWhitelist = string.IsNullOrEmpty(blockCommit.RoleWhitelist) ? new List<string>()
                                    : blockCommit.RoleWhitelist.Split(',').ToList(),
                            };
                            LoadResourceRules(blockCommit, result);
                            LoadWorkflowRules(blockCommit, result);
                            if (requestedBlock.ToolboxState != null)
                                LoadToolboxState(requestedBlock, result);
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

        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/lockBlock/{userId}")]
        [HttpGet]
        public bool LockBlock(int appId, int blockId,int userId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    TapestryDesignerBlock requestedBlock = context.TapestryDesignerBlocks.Find(blockId);
                    
                        requestedBlock.LockedForUserId = userId;
                        context.SaveChanges();

                        return true;
                 
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when locking block data (GET api/tapestry/apps/{appId}/blocks/{blockId}/{userId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/isBlockLocked/{userId}/commits/{currentCommitId}")]
        [HttpGet]
        public AjaxBlockLockingStatus isBlockLocked(int appId, int blockId, int userId,int currentCommitId)
        {
         
            using (var context = DBEntities.instance)
            {
                
                TapestryDesignerBlock requestedBlock = context.TapestryDesignerBlocks.Find(blockId);
                if (requestedBlock.LockedForUserId != null && userId != requestedBlock.LockedForUserId)
                {
                    string lockForUserName = context.Users.FirstOrDefault(u => u.Id == requestedBlock.LockedForUserId).DisplayName;
                    return new AjaxBlockLockingStatus() { lockStatusId = 1, lockedForUserName = lockForUserName };
                }
                else if (requestedBlock.LockedForUserId != null && userId == requestedBlock.LockedForUserId)
                {
                    TapestryDesignerBlockCommit lastBlockCommit = requestedBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                    
                    if (lastBlockCommit == null || (lastBlockCommit!=null && lastBlockCommit.Id == currentCommitId)) {//if last commit == current commit => OK
                        return new AjaxBlockLockingStatus() { lockStatusId = 2, lockedForUserName = "" };

                    }else
                    return new AjaxBlockLockingStatus() { lockStatusId = 3, lockedForUserName = "" };
                }
                else
                    return new AjaxBlockLockingStatus() { lockStatusId = 0, lockedForUserName = "" };
            }
        }

        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/unlockBlock")]
        [HttpGet]
        public bool UnlockBlock(int appId, int blockId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    TapestryDesignerBlock requestedBlock = context.TapestryDesignerBlocks.Find(blockId);
                    if (requestedBlock.LockedForUserId != null)
                    {
                        requestedBlock.LockedForUserId = null;
                        context.SaveChanges();
                        return true;

                    }
                    else {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when unlocking block data (GET api/tapestry/apps/{appId}/blocks/{blockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }



        [Route("api/tapestry/apps/{appId}/blocks/{blockId}")]
        [HttpPost]
        public int? SaveBlock(int appId, int blockId, AjaxTapestryDesignerBlockCommit postData)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    var app = context.Applications.Find(appId);
                    app.TapestryChangedSinceLastBuild = true;
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
                        ModelTableName = postData.ModelTableName,
                        AssociatedPageIds = postData.AssociatedPageIds != null ? string.Join(",", postData.AssociatedPageIds) : "",
                        AssociatedBootstrapPageIds = postData.AssociatedBootstrapPageIds != null ? string.Join(",", postData.AssociatedBootstrapPageIds) : "",
                        AssociatedTableName = postData.AssociatedTableName != null ? string.Join(",", postData.AssociatedTableName) : "",
                        AssociatedTableIds = postData.AssociatedTableIds != null ? string.Join(",", postData.AssociatedTableIds) : "",
                        RoleWhitelist = postData.RoleWhitelist != null ? string.Join(",", postData.RoleWhitelist) : "",
                    };
                    targetBlock.IsChanged = true;
                    targetBlock.LockedForUserId = null;
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
                                ActionId = ajaxItem.ActionId,
                                StateId = ajaxItem.StateId,
                                PageId = ajaxItem.IsBootstrap == true ? null : ajaxItem.PageId,
                                ComponentName = ajaxItem.ComponentName,
                                TableName = ajaxItem.TableName,
                                IsShared = ajaxItem.IsShared,
                                ColumnName = ajaxItem.ColumnName,
                                ColumnFilter = string.Join(",", ajaxItem.ColumnFilter.ToArray()),
                                IsBootstrap = ajaxItem.IsBootstrap,
                                BootstrapPageId = ajaxItem.IsBootstrap == true ? ajaxItem.BootstrapPageId : null
                            };
                            rule.ResourceItems.Add(item);
                            context.SaveChanges();
                            resourceIdMapping.Add(ajaxItem.Id, item.Id);
                            if (ajaxItem.ConditionSets.Count > 0)
                            {
                                TapestryDesignerConditionGroup conditionGroup = new TapestryDesignerConditionGroup
                                {
                                    ApplicationId = appId
                                };
                                item.ConditionGroups.Add(conditionGroup);
                                foreach (AjaxTapestryDesignerConditionSet ajaxConditionSet in ajaxItem.ConditionSets)
                                {
                                    TapestryDesignerConditionSet conditionSet = new TapestryDesignerConditionSet
                                    {
                                        SetIndex = ajaxConditionSet.SetIndex,
                                        SetRelation = ajaxConditionSet.SetRelation,
                                        ConditionGroup = conditionGroup
                                    };
                                    conditionGroup.ConditionSets.Add(conditionSet);

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
                        }
                        foreach (AjaxTapestryDesignerResourceConnection ajaxConnection in ajaxRule.Connections)
                        {
                            int source = resourceIdMapping[ajaxConnection.SourceId];
                            int target = resourceIdMapping[ajaxConnection.TargetId];
                            TapestryDesignerResourceConnection connection = new TapestryDesignerResourceConnection
                            {
                                SourceId = source,
                                SourceSlot = 0,
                                TargetId = target,
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

                            foreach(AjaxTapestryDesignerSubflow ajaxSubflow in ajaxSwimlane.Subflow) {
                                TapestryDesignerSubflow subflow = new TapestryDesignerSubflow
                                {
                                    Name = ajaxSubflow.Name,
                                    Comment = ajaxSubflow.Comment,
                                    CommentBottom = ajaxSubflow.CommentBottom,
                                    Width = ajaxSubflow.Width,
                                    Height = ajaxSubflow.Height,
                                    PositionX = ajaxSubflow.PositionX,
                                    PositionY = ajaxSubflow.PositionY,
                                };

                                swimlane.Subflow.Add(subflow);
                                context.SaveChanges();

                                foreach(AjaxTapestryDesignerWorkflowItem ajaxItem in ajaxSubflow.WorkflowItems) {
                                    TapestryDesignerWorkflowItem item = convertWorkflowItem(ajaxItem);
                                    item.ParentSwimlane = swimlane;
                                    subflow.WorkflowItems.Add(item);
                                    context.SaveChanges();

                                    workflowItemIdMapping.Add(ajaxItem.Id, item.Id);
                                    convertConditionSets(ref item, ajaxItem, appId);
                                }
                            }

                            foreach(AjaxTapestryDesignerForeach ajaxForeach in ajaxSwimlane.Foreach) {
                                TapestryDesignerForeach forEach = new TapestryDesignerForeach
                                {
                                    Name = ajaxForeach.Name,
                                    Comment = ajaxForeach.Comment,
                                    CommentBottom = ajaxForeach.CommentBottom,
                                    Width = ajaxForeach.Width,
                                    Height = ajaxForeach.Height,
                                    PositionX = ajaxForeach.PositionX,
                                    PositionY = ajaxForeach.PositionY,
                                    DataSource = ajaxForeach.DataSource
                                };
                                swimlane.Foreach.Add(forEach);
                                context.SaveChanges();

                                foreach(AjaxTapestryDesignerWorkflowItem ajaxItem in ajaxForeach.WorkflowItems) {
                                    TapestryDesignerWorkflowItem item = convertWorkflowItem(ajaxItem);
                                    item.ParentSwimlane = swimlane;
                                    forEach.WorkflowItems.Add(item);
                                    context.SaveChanges();

                                    workflowItemIdMapping.Add(ajaxItem.Id, item.Id);
                                    convertConditionSets(ref item, ajaxItem, appId);
                                }

                                TapestryDesignerWorkflowItem virtualItem = new TapestryDesignerWorkflowItem
                                {
                                    ActionId = null,
                                    Label = "Foreach virtual action",
                                    PositionX = ajaxForeach.PositionX,
                                    PositionY = ajaxForeach.PositionY,
                                    SymbolType = "foreach",
                                    TypeClass = "virtualAction",
                                    ParentSwimlane = swimlane,
                                    ParentForeach = forEach,
                                    InputVariables = "DataSource=" + forEach.DataSource
                                };
                                swimlane.WorkflowItems.Add(virtualItem);
                                context.SaveChanges();

                                workflowItemIdMapping.Add(ajaxForeach.Id, virtualItem.Id);
                            }
                            
                            foreach (AjaxTapestryDesignerWorkflowItem ajaxItem in ajaxSwimlane.WorkflowItems)
                            {
                                TapestryDesignerWorkflowItem item = convertWorkflowItem(ajaxItem);
                                swimlane.WorkflowItems.Add(item);
                                context.SaveChanges();

                                workflowItemIdMapping.Add(ajaxItem.Id, item.Id);
                                convertConditionSets(ref item, ajaxItem, appId);
                            }
                        }
                        foreach (AjaxTapestryDesignerWorkflowConnection ajaxConnection in ajaxRule.Connections)
                        {
                            int source = workflowItemIdMapping[ajaxConnection.SourceId];
                            int target = workflowItemIdMapping[ajaxConnection.TargetId];
                            TapestryDesignerWorkflowConnection connection = new TapestryDesignerWorkflowConnection
                            {
                                SourceId = source,
                                SourceSlot = ajaxConnection.SourceSlot,
                                TargetId = target,
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
                        TapestryDesignerMetablock nearbyMetablock = null;
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
                    if (postData.ToolboxState != null)
                    {
                        var toolboxState = new TapestryDesignerToolboxState();
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Actions)
                            toolboxState.Actions.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Attributes)
                            toolboxState.Attributes.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.UiComponents)
                            toolboxState.UiComponents.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Roles)
                            toolboxState.Roles.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.States)
                            toolboxState.States.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Targets)
                            toolboxState.Targets.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Templates)
                            toolboxState.Templates.Add(convertToolboxItem(ajaxToolboxItem));
                        foreach (var ajaxToolboxItem in postData.ToolboxState.Integrations)
                            toolboxState.Integrations.Add(convertToolboxItem(ajaxToolboxItem));
                        targetBlock.ToolboxState = toolboxState;
                    }
                    targetBlock.Name = postData.Name;
                    context.SaveChanges();

                    TapestryDesignerBlockCommit lastBlockCommit = targetBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                    return lastBlockCommit.Id;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Tapestry Designer: error when saving block data (POST api/tapestry/apps/{appId}/blocks/{blockId}). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        private static TapestryDesignerWorkflowItem convertWorkflowItem(AjaxTapestryDesignerWorkflowItem ajaxItem)
        {
            TapestryDesignerWorkflowItem item = new TapestryDesignerWorkflowItem
            {
                Label = ajaxItem.Label,
                Name = ajaxItem.Name,
                Comment = ajaxItem.Comment,
                CommentBottom = ajaxItem.CommentBottom,
                TypeClass = ajaxItem.TypeClass,
                DialogType = ajaxItem.DialogType,
                PositionX = ajaxItem.PositionX,
                PositionY = ajaxItem.PositionY,
                ActionId = ajaxItem.ActionId,
                InputVariables = ajaxItem.InputVariables,
                StateId = ajaxItem.StateId,
                TargetId = ajaxItem.TargetId,
                OutputVariables = ajaxItem.OutputVariables,
                PageId = ajaxItem.PageId,
                ComponentName = ajaxItem.ComponentName,
                isAjaxAction = ajaxItem.isAjaxAction,
                SymbolType = ajaxItem.SymbolType,
                IsBootstrap = ajaxItem.IsBootstrap,
                IsForeachStart = ajaxItem.IsForeachStart,
                IsForeachEnd = ajaxItem.IsForeachEnd
            };

            return item;
        }
        private static void convertConditionSets(ref TapestryDesignerWorkflowItem item, AjaxTapestryDesignerWorkflowItem ajaxItem, int AppId)
        {
            if (ajaxItem.ConditionSets.Count > 0)
            {
                TapestryDesignerConditionGroup conditionGroup = new TapestryDesignerConditionGroup
                {
                    ApplicationId = AppId
                };
                item.ConditionGroups.Add(conditionGroup);
                foreach (AjaxTapestryDesignerConditionSet ajaxConditionSet in ajaxItem.ConditionSets)
                {
                    TapestryDesignerConditionSet conditionSet = new TapestryDesignerConditionSet
                    {
                        SetIndex = ajaxConditionSet.SetIndex,
                        SetRelation = ajaxConditionSet.SetRelation,
                        ConditionGroup = conditionGroup
                    };
                    conditionGroup.ConditionSets.Add(conditionSet);
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
        }
        private static ToolboxItem convertToolboxItem(AjaxToolboxItem ajaxItem)
        {
            return new ToolboxItem
            {
                TypeClass = ajaxItem.TypeClass,
                Label = ajaxItem.Label,
                ActionId = ajaxItem.ActionId,
                TableName = ajaxItem.TableName,
                ColumnName = ajaxItem.ColumnName,
                PageId = ajaxItem.PageId,
                ComponentName = ajaxItem.ComponentName,
                StateId = ajaxItem.StateId,
                TargetName = ajaxItem.TargetName,
                TargetId = ajaxItem.TargetId,
                IsBootstrap = ajaxItem.IsBootstrap
            };
        }
        private static AjaxToolboxItem convertToolboxItem(ToolboxItem item)
        {
            return new AjaxToolboxItem
            {
                TypeClass = item.TypeClass,
                Label = item.Label,
                ActionId = item.ActionId,
                TableName = item.TableName,
                ColumnName = item.ColumnName,
                PageId = item.PageId,
                ComponentName = item.ComponentName,
                IsBootstrap = item.IsBootstrap,
                StateId = item.StateId,
                TargetName = item.TargetName,
                TargetId = item.TargetId
            };
        }
        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/commits")]
        [HttpGet]
        public IEnumerable<AjaxTapestryDesignerBlockCommitHeader> GetBlockHistory(int appId, int blockId)
        {
            try
            {
                using (var context = DBEntities.instance)
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
                using (var context = DBEntities.instance)
                {
                    TapestryDesignerBlockCommit blockCommit = context.TapestryDesignerBlocks.First(b => b.Id == blockId)
                        .BlockCommits.Where(c => c.Id == commitId).First();

                    AjaxTapestryDesignerBlockCommit result = new AjaxTapestryDesignerBlockCommit
                    {
                        Id = blockCommit.Id,
                        Name = blockCommit.Name,
                        PositionX = blockCommit.PositionX,
                        PositionY = blockCommit.PositionY,
                        Timestamp = blockCommit.Timestamp,
                        CommitMessage = blockCommit.CommitMessage,
                        ModelTableName = blockCommit.ModelTableName,
                        AssociatedPageIds = string.IsNullOrEmpty(blockCommit.AssociatedPageIds) ? new List<int>()
                                : blockCommit.AssociatedPageIds.Split(',').Select(int.Parse).ToList(),
                        AssociatedTableName = string.IsNullOrEmpty(blockCommit.AssociatedTableName) ? new List<string>()
                                : blockCommit.AssociatedTableName.Split(',').ToList(),
                        AssociatedTableIds = string.IsNullOrEmpty(blockCommit.AssociatedTableIds) ? new List<int>()
                                : blockCommit.AssociatedTableIds.Split(',').Select(int.Parse).ToList(),
                        RoleWhitelist = string.IsNullOrEmpty(blockCommit.RoleWhitelist) ? new List<string>()
                                : blockCommit.RoleWhitelist.Split(',').ToList(),
                        AssociatedBootstrapPageIds = string.IsNullOrEmpty(blockCommit.AssociatedBootstrapPageIds) ? new List<int>()
                                : blockCommit.AssociatedBootstrapPageIds.Split(',').Select(int.Parse).ToList(),
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
            using (var context = DBEntities.instance)
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
                using (var context = DBEntities.instance)
                {
                    var app = context.Applications.Find(appId);
                    app.TapestryChangedSinceLastBuild = true;
                    app.MenuChangedSinceLastBuild = true;
                    AjaxTapestryDesignerIdMapping idMapping = new AjaxTapestryDesignerIdMapping();
                    var targetMetablock = context.TapestryDesignerMetablocks.Find(metablockId);
                    if (targetMetablock == null)
                    {
                        targetMetablock = new TapestryDesignerMetablock
                        {
                            Id = appId
                        };
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
                            oldBlock.IsDeleted = false;
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
                            IsInMenu = ajaxBlock.IsInMenu,
                            IsChanged = true
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
                            oldMetablock.IsDeleted = false;
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
                            IsInMenu = ajaxMetablock.IsInMenu,
                            ParentAppId = appId
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
                using (var context = DBEntities.instance)
                {
                    var requestedMetablock = context.TapestryDesignerMetablocks.Find(metablockId);
                    var blockList = new List<AjaxTapestryDesignerBlock>();
                    var metablockList = new List<AjaxTapestryDesignerMetablock>();
                    var connectionList = new List<AjaxTapestryDesignerMetablockConnection>();

                    foreach (var sourceBlock in requestedMetablock.Blocks.Where(b => !b.IsDeleted))
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
                    foreach (var sourceMetablock in requestedMetablock.Metablocks.Where(mb => !mb.IsDeleted))
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
                using (DBEntities context = DBEntities.instance)
                {
                    foreach (KeyValuePair<int, int> row in data.Metablocks)
                    {
                        TapestryDesignerMetablock metablock = context.TapestryDesignerMetablocks.Where(m => m.Id == row.Key).First();
                        metablock.MenuOrder = row.Value;
                        metablock.ParentApp.MenuChangedSinceLastBuild = true;
                    }
                    foreach (KeyValuePair<int, int> row in data.Blocks)
                    {
                        TapestryDesignerBlock block = context.TapestryDesignerBlocks.First(b => b.Id == row.Key);
                        block.ParentMetablock.ParentApp.MenuChangedSinceLastBuild = true;
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

        [Route("api/tapestry/{appId}/blocks/{blockId}/copy/{targetMetablockId}")]
        [HttpGet]
        public bool CopyBlock(int appId, int blockId, int targetMetablockId)
        {
            DBEntities context = DBEntities.instance;

            try {
                TapestryDesignerBlock block = context.TapestryDesignerBlocks.FirstOrDefault(b => b.Id == blockId);
                TapestryDesignerMetablock target = context.TapestryDesignerMetablocks.FirstOrDefault(mb => mb.Id == targetMetablockId && mb.ParentAppId == appId);

                if (target == null || block == null) {
                    return false;
                }

                string blockName = block.Name;
                int i = 1;
                while(target.Blocks.Where(b => b.Name == blockName).Count() > 0) {
                    blockName = block.Name + " (" + i.ToString() + ")";
                    i++;
                }

                // create new block with modified values
                TapestryDesignerBlock clone = new TapestryDesignerBlock()
                {
                    AssociatedTableId = block.AssociatedTableId,
                    AssociatedTableName = block.AssociatedTableName,
                    BuiltBlockId = null,
                    IsChanged = true,
                    IsDeleted = false,
                    IsInitial = false,
                    IsInMenu = block.IsInMenu,
                    LockedForUserId = null,
                    MenuOrder = block.MenuOrder,
                    ParentMetablock = target,
                    PositionX = block.PositionX,
                    PositionY = block.PositionY,
                    Name = blockName
                };
                
                context.TapestryDesignerBlocks.Add(clone);
                
                // Set toolbox state
                TapestryDesignerToolboxState state = new TapestryDesignerToolboxState();
                foreach(ToolboxItem item in block.ToolboxState.Actions) {
                    state.Actions.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.Attributes) {
                    state.Attributes.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.Integrations) {
                    state.Integrations.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.Roles) {
                    state.Roles.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.States) {
                    state.States.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.Targets) {
                    state.Targets.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.Templates) {
                    state.Templates.Add(CloneToolboxItem(item));
                }
                foreach(ToolboxItem item in block.ToolboxState.UiComponents) {
                    state.UiComponents.Add(CloneToolboxItem(item));
                }

                // get last commit and create new one
                TapestryDesignerBlockCommit commit = CloneLastCommit(block, clone, blockName, false);
                commit.Name = blockName;
                commit.CommitMessage = string.Format("Copy {0} from {1} metablock", block.Name, block.ParentMetablock.Name);

                clone.ToolboxState = state;
                clone.BlockCommits.Add(commit);
                
                context.SaveChanges();
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        [Route("api/tapestry/{appId}/blocks/{blockId}/move/{targetMetablockId}")]
        [HttpGet]
        public bool MoveBlock(int appId, int blockId, int targetMetablockId)
        {
            DBEntities context = DBEntities.instance;

            try {
                TapestryDesignerBlock block = context.TapestryDesignerBlocks.FirstOrDefault(b => b.Id == blockId);
                TapestryDesignerMetablock target = context.TapestryDesignerMetablocks.FirstOrDefault(mb => mb.Id == targetMetablockId && mb.ParentAppId == appId);

                if (target == null || block == null) {
                    return false;
                }

                string tmpName = block.Name;
                int i = 1;
                while(target.Blocks.Where(b => b.Name == tmpName).Count() > 0) {
                    tmpName = block.Name + " (" + i.ToString() + ")";
                    i++;
                }

                block.ParentMetablock = target;
                block.Name = tmpName;

                TapestryDesignerBlockCommit commit = CloneLastCommit(block, block, tmpName, false);
                commit.Name = tmpName;
                commit.CommitMessage = string.Format("Move {0} from {1} metablock", block.Name, block.ParentMetablock.Name);

                block.BlockCommits.Add(commit);
                
                context.SaveChanges();
            }
            catch(Exception) {
                return false;
            }

            return true;
        }

        [Route("api/tapestry/apps/{appId}/blocks/{blockId}/copyWorkflow/{sourceId}/target/{targetBlockId}")]
        [HttpGet]
        public bool CopyWorkflow(int appId, int blockId, int sourceId, int targetBlockId)
        {
            DBEntities context = DBEntities.instance;

            Application app = context.Applications.FirstOrDefault(a => a.Id == appId);
            TapestryDesignerWorkflowRule source = context.TapestryDesignerWorkflowRules.FirstOrDefault(w => w.Id == sourceId);
            TapestryDesignerBlock target = context.TapestryDesignerBlocks.FirstOrDefault(b => b.Id == targetBlockId && b.ParentMetablock.ParentAppId == appId);
            TapestryDesignerBlock block = context.TapestryDesignerBlocks.FirstOrDefault(b => b.Id == blockId && b.ParentMetablock.ParentAppId == appId);

            if(source == null || target == null || block == null) {
                return false;
            }

            try {
                TapestryDesignerBlockCommit commit = CloneLastCommit(target, null, null, true);
                commit.CommitMessage = string.Format("Copy workflow \"{0}\" from block \"{1}\"", source.Name, block.Name);

                string tmpName = source.Name;
                int i = 1;
                while (commit.WorkflowRules.Where(w => w.Name == tmpName).Count() > 0) {
                    tmpName = source.Name + " (" + i.ToString() + ")";
                    i++;
                }

                TapestryDesignerWorkflowRule bottomRule = null;
                int top = 0;
                foreach (TapestryDesignerWorkflowRule r in commit.WorkflowRules) {
                    if (r.PositionY > top) {
                        top = r.PositionY;
                        bottomRule = r;
                    }
                }

                TapestryDesignerWorkflowRule newWR = new TapestryDesignerWorkflowRule()
                {
                    Height = source.Height,
                    Name = source.Name,
                    PositionX = bottomRule == null ? 10 : bottomRule.PositionX,
                    PositionY = bottomRule == null ? 10 : bottomRule.PositionY + bottomRule.Height + 10,
                    Width = source.Width
                };

                commit.WorkflowRules.Add(newWR);
                CloneWorkflowContent(source, ref newWR, target, null, null, true);

                target.BlockCommits.Add(commit);

                context.SaveChanges();
                return true;
            }
            catch(Exception) { }

            return false;
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
            var metablock = requestedApp.TapestryDesignerRootMetablock;
            var ajaxMetablock = new AjaxTapestryDesignerMetablock
            {
                Id = metablock.Id,
                Name = metablock.Name,
                PositionX = metablock.PositionX,
                PositionY = metablock.PositionY
            };
            LoadMetablocks(metablock, ajaxMetablock);
            result.RootMetablock = ajaxMetablock;
        }
        private static void LoadMetablocks(TapestryDesignerMetablock requestedMetablock, AjaxTapestryDesignerMetablock result)
        {
            foreach (TapestryDesignerMetablock metablock in requestedMetablock.Metablocks.Where(mb => !mb.IsDeleted))
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
            foreach (TapestryDesignerBlock block in requestedMetablock.Blocks.Where(b => !b.IsDeleted))
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
                    ActionId = item.ActionId,
                    StateId = item.StateId,
                    PageId = item.PageId,
                    ComponentName = item.ComponentName,
                    IsBootstrap = item.IsBootstrap,
                    BootstrapPageId = item.BootstrapPageId,
                    TableName = item.TableName,
                    IsShared = item.IsShared,
                    ColumnName = item.ColumnName,
                    ColumnFilter = string.IsNullOrEmpty(item.ColumnFilter) ? new List<string>() : item.ColumnFilter.Split(',').ToList()
                };
                LoadConditionSets(item, ajaxItem);
                result.ResourceItems.Add(ajaxItem);
            }
        }
        private static void LoadConditionSets(TapestryDesignerResourceItem requestedItem, AjaxTapestryDesignerResourceItem result)
        {
            if (requestedItem.ConditionGroups.Count > 0)
                foreach (TapestryDesignerConditionSet conditionSet in requestedItem.ConditionGroups.First().ConditionSets)
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
        private static void LoadConditionSets(TapestryDesignerWorkflowItem requestedItem, AjaxTapestryDesignerWorkflowItem result)
        {
            if (requestedItem.ConditionGroups.Count > 0)
                foreach (TapestryDesignerConditionSet conditionSet in requestedItem.ConditionGroups.First().ConditionSets)
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
                LoadSubflow(swimlane, ajaxSwimlane);
                LoadForeach(swimlane, ajaxSwimlane);
                LoadWorkflowItems(swimlane, ajaxSwimlane);
                result.Swimlanes.Add(ajaxSwimlane);
            }
        }
        private static void LoadSubflow(TapestryDesignerSwimlane requestedSwimlane, AjaxTapestryDesignerSwimlane result)
        {
            foreach (TapestryDesignerSubflow subflow in requestedSwimlane.Subflow) {
                var ajaxSubflow = new AjaxTapestryDesignerSubflow
                {
                    Id = subflow.Id,
                    Name = subflow.Name,
                    Comment = subflow.Comment,
                    CommentBottom = subflow.CommentBottom,
                    Height = subflow.Height,
                    Width = subflow.Width,
                    PositionX = subflow.PositionX,
                    PositionY = subflow.PositionY
                };
                result.Subflow.Add(ajaxSubflow);
            }
        }
        private static void LoadForeach(TapestryDesignerSwimlane requestedSwimlane, AjaxTapestryDesignerSwimlane result)
        {
            foreach (TapestryDesignerForeach forEach in requestedSwimlane.Foreach) {
                var ajaxForeach = new AjaxTapestryDesignerForeach
                {
                    Id = forEach.Id,
                    Name = forEach.Name,
                    Comment = forEach.Comment,
                    CommentBottom = forEach.CommentBottom,
                    Height = forEach.Height,
                    Width = forEach.Width,
                    PositionX = forEach.PositionX,
                    PositionY = forEach.PositionY,
                    DataSource = forEach.DataSource
                };
                result.Foreach.Add(ajaxForeach);
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
                    Name = item.Name,
                    Comment = item.Comment,
                    CommentBottom = item.CommentBottom,
                    TypeClass = item.TypeClass,
                    DialogType = item.DialogType,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    ActionId = item.ActionId,
                    InputVariables = item.InputVariables,
                    OutputVariables = item.OutputVariables,
                    StateId = item.StateId,
                    PageId = item.PageId,
                    ComponentName = item.ComponentName,
                    IsBootstrap = item.IsBootstrap,
                    TargetId = item.TargetId,
                    isAjaxAction = item.isAjaxAction,
                    SymbolType = item.SymbolType,
                    ParentSubflowId = item.ParentSubflowId,
                    ParentForeachId = item.ParentForeachId,
                    IsForeachStart = item.IsForeachStart,
                    IsForeachEnd = item.IsForeachEnd
                };
                LoadConditionSets(item, ajaxItem);
                result.WorkflowItems.Add(ajaxItem);
            }
        }
        private static void LoadConnections(TapestryDesignerResourceRule requestedRule, AjaxTapestryDesignerResourceRule result)
        {
            foreach (TapestryDesignerResourceConnection connection in requestedRule.Connections)
            {
                var ajaxConnection = new AjaxTapestryDesignerResourceConnection
                {
                    Id = connection.Id,
                    SourceId = connection.SourceId,
                    SourceSlot = connection.SourceSlot,
                    TargetId = connection.TargetId,
                    TargetSlot = connection.TargetSlot
                };
                result.Connections.Add(ajaxConnection);
            }
        }
        private static void LoadConnections(TapestryDesignerWorkflowRule requestedRule, AjaxTapestryDesignerWorkflowRule result)
        {
            foreach (TapestryDesignerWorkflowConnection connection in requestedRule.Connections)
            {
                var ajaxConnection = new AjaxTapestryDesignerWorkflowConnection
                {
                    Id = connection.Id,
                    SourceId = connection.SourceId,
                    SourceSlot = connection.SourceSlot,
                    TargetId = connection.TargetId,
                    TargetSlot = connection.TargetSlot
                };
                result.Connections.Add(ajaxConnection);
            }
        }
        private static void LoadToolboxState(TapestryDesignerBlock requestedBlock, AjaxTapestryDesignerBlockCommit result)
        {
            var ajaxToolboxState = new AjaxTapestryDesignerToolboxState();
            var blockToolboxState = requestedBlock.ToolboxState;
            foreach (var toolboxItem in blockToolboxState.Actions)
                ajaxToolboxState.Actions.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.Attributes)
                ajaxToolboxState.Attributes.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.UiComponents)
                ajaxToolboxState.UiComponents.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.Roles)
                ajaxToolboxState.Roles.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.States)
                ajaxToolboxState.States.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.Targets)
                ajaxToolboxState.Targets.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.Templates)
                ajaxToolboxState.Templates.Add(convertToolboxItem(toolboxItem));
            foreach (var toolboxItem in blockToolboxState.Integrations)
                ajaxToolboxState.Integrations.Add(convertToolboxItem(toolboxItem));
            result.ToolboxState = ajaxToolboxState;
        }
        private static void DeleteMetablock(TapestryDesignerMetablock metablockToDelete, DBEntities context)
        {
            if (!metablockToDelete.IsDeleted)
            {
                metablockToDelete.IsDeleted = true;
                metablockToDelete.Name = $"{metablockToDelete.Name}-{DateTime.UtcNow.ToString()}";
            }   
        }
        private static void DeleteBlock(TapestryDesignerBlock blockToDelete, DBEntities context)
        {
            if (!blockToDelete.IsDeleted)
            {
                blockToDelete.IsDeleted = true;
                blockToDelete.IsChanged = true;
                blockToDelete.Name = $"{blockToDelete.Name}-{DateTime.UtcNow.ToString()}";
            }
        }
        private void CollectBlocksToList(TapestryDesignerMetablock rootMetablock,
            AjaxTapestryDesignerBlockList list, DBEntities context)
        {
            foreach (TapestryDesignerBlock block in rootMetablock.Blocks.Where(b => !b.IsDeleted))
                list.ListItems.Add(new AjaxTapestryDesignerBlockListItem
                {
                    Id = block.Id,
                    Name = block.Name
                });
            foreach (TapestryDesignerMetablock metablock in rootMetablock.Metablocks.Where(mb => !mb.IsDeleted))
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

        private TapestryDesignerBlockCommit CloneLastCommit(TapestryDesignerBlock block, TapestryDesignerBlock clone, string newBlockName, bool move)
        {
            TapestryDesignerBlockCommit lastCommit = block.BlockCommits.OrderByDescending(c => c.Id).FirstOrDefault();
            TapestryDesignerBlockCommit commit = new TapestryDesignerBlockCommit();

            // if last commit exists, set values
            if (lastCommit != null) {
                commit.AssociatedBootstrapPageIds = lastCommit.AssociatedBootstrapPageIds;
                commit.AssociatedPageIds = lastCommit.AssociatedPageIds;
                commit.AssociatedTableIds = lastCommit.AssociatedTableIds;
                commit.AssociatedTableName = lastCommit.AssociatedTableName;
                commit.ModelTableName = lastCommit.ModelTableName;
                commit.PositionX = lastCommit.PositionX;
                commit.PositionY = lastCommit.PositionY;
                commit.RoleWhitelist = lastCommit.RoleWhitelist;
                commit.Timestamp = DateTime.Now;
                commit.Name = lastCommit.Name;

                // set commit resource rules
                foreach (TapestryDesignerResourceRule rr in lastCommit.ResourceRules) {
                    TapestryDesignerResourceRule newRR = new TapestryDesignerResourceRule()
                    {
                        Height = rr.Height,
                        ParentBlockCommit = commit,
                        PositionX = rr.PositionX,
                        PositionY = rr.PositionY,
                        Width = rr.Width
                    };

                    commit.ResourceRules.Add(newRR);

                    // set commit resource rule resource items
                    foreach (TapestryDesignerResourceItem ri in rr.ResourceItems) {
                        TapestryDesignerResourceItem newRI = new TapestryDesignerResourceItem()
                        {
                            ActionId = ri.ActionId,
                            BootstrapPage = ri.BootstrapPage,
                            ColumnFilter = ri.ColumnFilter,
                            ColumnName = ri.ColumnName,
                            ComponentName = ri.ComponentName,
                            IsBootstrap = ri.IsBootstrap,
                            IsShared = ri.IsShared,
                            Label = ri.Label,
                            Page = ri.Page,
                            PositionX = ri.PositionX,
                            PositionY = ri.PositionY,
                            TableName = ri.TableName,
                            StateId = ri.StateId,
                            TypeClass = ri.TypeClass
                        };

                        newRR.ResourceItems.Add(newRI);

                        // set resource item condition sets
                        foreach (TapestryDesignerConditionGroup cg in ri.ConditionGroups)
                        {
                            TapestryDesignerConditionGroup newCg = new TapestryDesignerConditionGroup
                            {
                                ApplicationId = block.ParentMetablock.ParentAppId
                            };
                            newRI.ConditionGroups.Add(newCg);

                            foreach (TapestryDesignerConditionSet cs in cg.ConditionSets)
                            {
                                TapestryDesignerConditionSet newCs = new TapestryDesignerConditionSet
                                {
                                    SetIndex = cs.SetIndex,
                                    SetRelation = cs.SetRelation
                                };
                                newCg.ConditionSets.Add(newCs);

                                // set condition set conditions
                                foreach (TapestryDesignerCondition c in cs.Conditions)
                                {
                                    TapestryDesignerCondition newC = new TapestryDesignerCondition()
                                    {
                                        Index = c.Index,
                                        Operator = c.Operator,
                                        Relation = c.Relation,
                                        Value = c.Value,
                                        Variable = c.Variable
                                    };
                                    newCs.Conditions.Add(newC);
                                }
                            }
                        }
                    }

                    // set resource item connection
                    foreach (TapestryDesignerResourceConnection c in rr.Connections) {
                        TapestryDesignerResourceConnection newC = new TapestryDesignerResourceConnection()
                        {
                            SourceSlot = c.SourceSlot,
                            TargetSlot = c.TargetSlot,
                            Source = newRR.ResourceItems.Where(ci => ci.PositionX == c.Source.PositionX && ci.PositionY == c.Source.PositionY && ci.Label == c.Source.Label).FirstOrDefault(),
                            Target = newRR.ResourceItems.Where(ci => ci.PositionX == c.Target.PositionX && ci.PositionY == c.Target.PositionY && ci.Label == c.Target.Label).FirstOrDefault()
                        };

                        newRR.Connections.Add(newC);
                    }
                }

                // set commit workflow rules
                foreach (TapestryDesignerWorkflowRule wr in lastCommit.WorkflowRules) {
                    TapestryDesignerWorkflowRule newWR = new TapestryDesignerWorkflowRule()
                    {
                        Height = wr.Height,
                        Name = wr.Name,
                        PositionX = wr.PositionX,
                        PositionY = wr.PositionY,
                        Width = wr.Width
                    };

                    commit.WorkflowRules.Add(newWR);
                    CloneWorkflowContent(wr, ref newWR, block, clone, newBlockName, move);
                }
            }

            return commit;
        }

        private void CloneWorkflowContent(TapestryDesignerWorkflowRule source, ref TapestryDesignerWorkflowRule target, TapestryDesignerBlock block, TapestryDesignerBlock clone, string newBlockName, bool move)
        {
            // Set workflow rule swimlanes
            List<TapestryDesignerWorkflowItem> allItems = new List<TapestryDesignerWorkflowItem>();
            foreach (TapestryDesignerSwimlane sl in source.Swimlanes) {
                TapestryDesignerSwimlane newSL = new TapestryDesignerSwimlane()
                {
                    Height = sl.Height,
                    Roles = sl.Roles,
                    SwimlaneIndex = sl.SwimlaneIndex,
                };

                target.Swimlanes.Add(newSL);

                // copy subflow
                Dictionary<int, TapestryDesignerSubflow> subflowMapping = new Dictionary<int, TapestryDesignerSubflow>();
                foreach(TapestryDesignerSubflow sf in sl.Subflow) {
                    TapestryDesignerSubflow newSF = new TapestryDesignerSubflow
                    {
                        Name = sf.Name,
                        Comment = sf.Comment,
                        CommentBottom = sf.CommentBottom,
                        Height = sf.Height,
                        Width = sf.Width,
                        PositionX = sf.PositionX,
                        PositionY = sf.PositionY,
                    };

                    newSL.Subflow.Add(newSF);
                    subflowMapping.Add(sf.Id, newSF);
                }

                // copy foreach
                Dictionary<int, TapestryDesignerForeach> foreachMapping = new Dictionary<int, TapestryDesignerForeach>();
                foreach (TapestryDesignerForeach fe in sl.Foreach) {
                    TapestryDesignerForeach newFE = new TapestryDesignerForeach
                    {
                        Name = fe.Name,
                        Comment = fe.Comment,
                        CommentBottom = fe.CommentBottom,
                        Height = fe.Height,
                        Width = fe.Width,
                        PositionX = fe.PositionX,
                        PositionY = fe.PositionY,
                        DataSource = fe.DataSource
                    };

                    newSL.Foreach.Add(newFE);
                    foreachMapping.Add(fe.Id, newFE);
                }

                // set swimlane workflow items
                foreach (TapestryDesignerWorkflowItem wi in sl.WorkflowItems) {
                    TapestryDesignerWorkflowItem newWI = new TapestryDesignerWorkflowItem()
                    {
                        ActionId = wi.ActionId,
                        Comment = wi.Comment,
                        CommentBottom = wi.CommentBottom,
                        ComponentName = wi.ComponentName,
                        Condition = wi.Condition,
                        DialogType = wi.DialogType,
                        InputVariables = wi.InputVariables,
                        isAjaxAction = wi.isAjaxAction,
                        IsBootstrap = wi.IsBootstrap,
                        IsForeachStart = wi.IsForeachStart,
                        IsForeachEnd = wi.IsForeachEnd,
                        Label = wi.Label,
                        Name = wi.Name,
                        OutputVariables = wi.OutputVariables,
                        PageId = wi.PageId,
                        PositionX = wi.PositionX,
                        PositionY = wi.PositionY,
                        StateId = wi.StateId,
                        SymbolType = wi.SymbolType,
                        Target = wi.Target != null && wi.Target.Id == block.Id && !move ? clone : wi.Target,
                        TargetName = wi.Target != null && wi.Target.Id == block.Id && !move ? newBlockName : wi.TargetName,
                        TypeClass = wi.TypeClass
                    };

                    newSL.WorkflowItems.Add(newWI);
                    allItems.Add(newWI);

                    if(wi.ParentSubflowId != null) {
                        subflowMapping[(int)wi.ParentSubflowId].WorkflowItems.Add(newWI);
                    }
                    if (wi.ParentForeachId != null) {
                        foreachMapping[(int)wi.ParentForeachId].WorkflowItems.Add(newWI);
                    }

                    // set workflow item condition sets
                    foreach (TapestryDesignerConditionGroup cg in wi.ConditionGroups)
                    {
                        TapestryDesignerConditionGroup newcg = new TapestryDesignerConditionGroup
                        {
                            ApplicationId = block.ParentMetablock.ParentAppId
                        };
                        newWI.ConditionGroups.Add(newcg);
                        foreach (TapestryDesignerConditionSet cs in cg.ConditionSets)
                        {
                            TapestryDesignerConditionSet newCS = new TapestryDesignerConditionSet()
                            {
                                SetIndex = cs.SetIndex,
                                SetRelation = cs.SetRelation
                            };
                            newcg.ConditionSets.Add(newCS);
                            // set conditions in set
                            foreach (TapestryDesignerCondition c in cs.Conditions)
                            {
                                TapestryDesignerCondition newC = new TapestryDesignerCondition()
                                {
                                    Index = c.Index,
                                    Operator = c.Operator,
                                    Relation = c.Relation,
                                    Value = c.Value,
                                    Variable = c.Variable
                                };
                                newCS.Conditions.Add(newC);
                            }
                        }
                    }
                }
            }

            // Set workflow rule connections
            foreach (TapestryDesignerWorkflowConnection wc in source.Connections) {
                TapestryDesignerWorkflowConnection newWC = new TapestryDesignerWorkflowConnection()
                {
                    SourceSlot = wc.SourceSlot,
                    TargetSlot = wc.TargetSlot,
                    Source = allItems.Where(m => m.PositionX == wc.Source.PositionX && m.PositionY == wc.Source.PositionY && m.TypeClass == wc.Source.TypeClass && m.Label == wc.Source.Label).FirstOrDefault(),
                    Target = allItems.Where(m => m.PositionX == wc.Target.PositionX && m.PositionY == wc.Target.PositionY && m.TypeClass == wc.Target.TypeClass && m.Label == wc.Target.Label).FirstOrDefault()
                };

                target.Connections.Add(newWC);
            }
        }

        private ToolboxItem CloneToolboxItem(ToolboxItem item)
        {
            ToolboxItem newTI = new ToolboxItem()
            {
                ActionId = item.ActionId,
                ColumnName = item.ComponentName,
                ComponentName = item.ComponentName,
                IsBootstrap = item.IsBootstrap,
                Label = item.Label,
                PageId = item.PageId,
                StateId = item.StateId,
                TableName = item.TableName,
                TargetId = item.TargetId,
                TargetName = item.TargetName,
                TypeClass = item.TypeClass
            };
            return newTI;
        }

        public TapestryApiController() { }
    }
}
