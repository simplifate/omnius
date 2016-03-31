using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Service
{
    public class TapestryGeneratorService
    {
        private CORE.CORE _core;
        private DBEntities _context;

        private Dictionary<int, Block> _blockMapping;

        public TapestryGeneratorService()
        {
            _blockMapping = new Dictionary<int, Block>();
        }

        public Dictionary<int, Block> GenerateTapestry(CORE.CORE core)
        {
            _core = core;
            _context = core.Entitron.GetStaticTables();
            Application app = core.Entitron.Application;

            // remove old temp blocks - should do nothing
            _context.WorkFlows.RemoveRange(app.WorkFlows.Where(w => w.IsTemp));
            _context.SaveChanges();

            try
            {
                // generate new
                WorkFlow wf = saveMetaBlock(app.TapestryDesignerRootMetablock, true);
                _context.SaveChanges();
                
                // remove old
                _context.WorkFlows.RemoveRange(app.WorkFlows.Where(w => !w.IsTemp));
                _context.SaveChanges();

                foreach (WorkFlow workflow in app.WorkFlows)
                    workflow.IsTemp = false;
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _context.WorkFlows.RemoveRange(app.WorkFlows.Where(w => w.IsTemp));
                _context.SaveChanges();

                throw ex;
            }

            return _blockMapping;
        }

        private WorkFlow saveMetaBlock(TapestryDesignerMetablock block, bool init = false)
        {
            WorkFlow resultWF = new WorkFlow
            {
                ApplicationId = _core.Entitron.AppId,
                Name = block.Name.RemoveDiacritics(),
                Type = init ? _context.WorkFlowTypes.Single(t => t.Name == "Init") : _context.WorkFlowTypes.Single(t => t.Name == "Partial"),
                IsTemp = true
            };
            _context.WorkFlows.Add(resultWF);
            _context.SaveChanges();

            // child meta block
            foreach (TapestryDesignerMetablock childMetaBlock in block.Metablocks)
            {
                WorkFlow wf = saveMetaBlock(childMetaBlock);
                wf.Parent = resultWF;
            }
            _context.SaveChanges();

            // child block
            foreach (TapestryDesignerBlock childBlock in block.Blocks)
            {
                TapestryDesignerBlockCommit commit = childBlock.BlockCommits.OrderByDescending(c => c.Timestamp).FirstOrDefault();
                string modelName = commit != null ? commit.AssociatedTableName : null;

                Block resultBlock = new Block
                {
                    Name = childBlock.Name.RemoveDiacritics(),
                    DisplayName = childBlock.Name,
                    ModelName = modelName != null ? modelName.Split(',').First() : null,
                    IsVirtual = false
                };
                resultWF.Blocks.Add(resultBlock);
                if (childBlock.IsInitial)
                    resultBlock.InitForWorkFlow.Add(resultWF);

                _blockMapping.Add(childBlock.Id, resultBlock);
            }
            foreach (TapestryDesignerBlock childBlock in block.Blocks)
            {
                try
                {
                    saveBlockContent(childBlock, resultWF);
                }
                catch(Exception e)
                {
                    throw new Exception($"block [{childBlock.Name}] - {e.Message}", e);
                }
            }
            _context.SaveChanges();

            // DONE :)
            return resultWF;
        }

        private void saveBlockContent(TapestryDesignerBlock block, WorkFlow wf)
        {
            // block
            Block resultBlock = _blockMapping[block.Id];

            TapestryDesignerBlockCommit commit = block.BlockCommits.OrderBy(bc => bc.Timestamp).LastOrDefault();
            if (commit == null) // no commit
                return;
            // Resources
            foreach (TapestryDesignerResourceRule resourceRule in commit.ResourceRules)
            {
                var pair = saveResourceRule(resourceRule, wf.Application);
                resultBlock.ResourceMappingPairs.Add(pair);
            }

            // ActionRule
            foreach (TapestryDesignerWorkflowRule workflowRule in commit.WorkflowRules)
            {
                saveWFRule(workflowRule, resultBlock, wf);
            }

            if (commit.AssociatedPageIds != "")
            {
                var pageIdList = commit.AssociatedPageIds.Split(',').Select(int.Parse).ToList();
                Page mainPage = null;
                foreach (int pageId in pageIdList)
                {
                    var currentPage = _context.MozaicEditorPages.Find(pageId);
                    if (!currentPage.IsModal)
                    {
                        mainPage = _context.Pages.Find(currentPage.CompiledPageId);
                        break;
                    }
                }
                resultBlock.MozaicPage = mainPage;
            }
        }

        private ResourceMappingPair saveResourceRule(TapestryDesignerResourceRule resourceRule, Application app)
        {
            AttributeRule result = new AttributeRule();
            using (var context = new DBEntities())
            {
                foreach (TapestryDesignerResourceConnection connection in resourceRule.Connections)
                {
                    TapestryDesignerResourceItem source = connection.Source;
                    TapestryDesignerResourceItem target = connection.Target;

                    string targetName = "", targetType = "", dataSourceParams = "";

                    if (source.ActionId == 1023)
                        continue;

                    if (!string.IsNullOrEmpty(target.ComponentName))
                    {
                        var targetPage = context.MozaicEditorPages.Find(target.PageId);
                        var component = targetPage.Components.SingleOrDefault(c => c.Name == target.ComponentName);
                        if (component == null)
                        {
                            foreach (var parentComponent in targetPage.Components)
                            {
                                if (parentComponent.ChildComponents.Count > 0)
                                    component = parentComponent.ChildComponents.SingleOrDefault(c => c.Name == target.ComponentName);
                                if (component != null)
                                    break;
                            }
                        }
                        if (component != null)
                        {
                            targetName = component.Name;
                            targetType = component.Type;
                        }
                    }
                    if (!string.IsNullOrEmpty(source.ColumnName))
                    {
                        foreach (TapestryDesignerResourceConnection relatedConnection in resourceRule.Connections)
                        {
                            if (relatedConnection.TargetId == source.Id)
                            {
                                if (relatedConnection.Source.ActionId == 1023)
                                    dataSourceParams = "currentUser";
                            }
                        }
                    }
                    return new ResourceMappingPair
                    {
                        Source = source,
                        Target = target,
                        TargetName = targetName,
                        TargetType = targetType,
                        SourceColumnFilter = source.ColumnFilter,
                        DataSourceParams = dataSourceParams
                    };
                }
            }
            return null;
        }

        private void saveWFRule(TapestryDesignerWorkflowRule workflowRule, Block block, WorkFlow wf)
        {
            HashSet<TapestryDesignerWorkflowConnection> todoConnections = new HashSet<TapestryDesignerWorkflowConnection>();
            Dictionary<Block, string> conditionMapping = new Dictionary<Block, string>();
            Dictionary<TapestryDesignerWorkflowItem, Block> BlockMapping = new Dictionary<TapestryDesignerWorkflowItem, Block>();

            var splitItems = workflowRule.Connections.GroupBy(c => c.SourceId).Where(c => c.Count() > 1);
            var joinItems = workflowRule.Connections.GroupBy(c => c.TargetId).Where(c => c.Count() > 1);

            foreach (var splitItem in splitItems)
            {
                // todo connection
                foreach (TapestryDesignerWorkflowConnection connection in splitItem)
                {
                    todoConnections.Add(connection);
                }

                // block mapping
                int random = new Random().Next() % 1000000;
                Block newBlock = new Block
                {
                    Name = $"split_{block.Name}_{random}",
                    DisplayName = $"split[{block.Name}_{random}]",
                    ModelName = block.ModelName,
                    IsVirtual = true
                };
                wf.Blocks.Add(newBlock);
                TapestryDesignerWorkflowItem it = _context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == splitItem.Key);
                BlockMapping.Add(it, newBlock);

                // conditions
                if (it.TypeClass == "gateway-x")
                {
                    conditionMapping.Add(newBlock, it.Condition);
                }
            }
            foreach (var joinItem in joinItems)
            {
                // todo connection
                todoConnections.Add(joinItem.FirstOrDefault());

                // block mapping
                int random = new Random().Next() % 1000000;
                Block newBlock = new Block
                {
                    Name = $"join_{block.Name}_{random}",
                    DisplayName = $"join[{block.Name}_{random}]",
                    ModelName = block.ModelName,
                    IsVirtual = true
                };
                wf.Blocks.Add(newBlock);

                foreach (var ji in joinItem)
                {

                    TapestryDesignerWorkflowItem it = _context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == ji.SourceId);
                    BlockMapping.Add(it, newBlock);
                }
            }

            // begin
            TapestryDesignerWorkflowItem item = _context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.TypeClass == "uiItem");
            if (item == null)
                return;
            createActionRule(workflowRule, block, new TapestryDesignerWorkflowConnection { Target = item }, BlockMapping, conditionMapping, item.ComponentName);


            //// ACTIONS ////
            foreach (TapestryDesignerWorkflowConnection conection in todoConnections)
            {
                TapestryDesignerWorkflowItem it = conection.Source;
                Block thisBlock = BlockMapping[it];
                createActionRule(workflowRule, thisBlock, conection, BlockMapping, conditionMapping);
            }
        }

        private ActionRule createActionRule(TapestryDesignerWorkflowRule workflowRule, Block startBlock, TapestryDesignerWorkflowConnection connection,
            Dictionary<TapestryDesignerWorkflowItem, Block> blockMapping, Dictionary<Block, string> conditionMapping, string init = null)
        {
            string ActorName = (init != null ? "Manual" : "Auto");
            ActionRule rule = new ActionRule
            {
                Actor = _context.Actors.Single(a => a.Name == ActorName),
                Name = (new Random().Next() % 1000000).ToString(),
                ExecutedBy = init
            };
            // condition
            if (conditionMapping.ContainsKey(startBlock))
            {
                rule.Condition = connection.SourceSlot == 0 ? conditionMapping[startBlock] : $"!{conditionMapping[startBlock]}";
            }
            startBlock.SourceTo_ActionRules.Add(rule);
            // rights
            AddActionRuleRights(rule, connection.Target.ParentSwimlane);

            TapestryDesignerWorkflowItem item = connection.Target;
            TapestryDesignerWorkflowItem prevItem = null;
            while (item != null && (prevItem == null || !blockMapping.ContainsKey(prevItem)))
            {
                switch(item.TypeClass)
                {
                    case "actionItem":
                        ActionRule_Action result = new ActionRule_Action
                        {
                            ActionId = item.ActionId.Value,
                            Order = rule.ActionRule_Actions.Any() ? rule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1,
                            InputVariablesMapping = item.InputVariables,
                            OutputVariablesMapping = item.OutputVariables
                        };
                        rule.ActionRule_Actions.Add(result);
                        break;

                    case "targetItem":
                        rule.TargetBlock = _blockMapping[item.TargetId.Value];
                        break;

                    case "gateway-x":
                        Block splitBlock = blockMapping[item];
                        // if not already in conditionMapping
                        if (!conditionMapping.ContainsKey(splitBlock))
                            conditionMapping.Add(splitBlock, item.Condition);
                        break;

                    case "circle-thick":
                        break;
                    case "attributeItem":
                        break;
                    case "templateItem":
                        break;
                    case "stateItem":
                        break;
                    case "circle-single":
                        break;
                    case "circle-single-dashed":
                        break;
                    case "uiItem":
                        break;
                    case "database":
                        break;
                    case "comment":
                        break;
                    default:
                        break;
                }

                // next connection
                connection = workflowRule.Connections.FirstOrDefault(c => c.SourceId == item.Id);
                prevItem = item;
                item = connection != null ? connection.Target : null;
            }

            if (rule.TargetBlock == null)
            {
                if (blockMapping.ContainsKey(prevItem))
                    rule.TargetBlock = blockMapping[prevItem];
                else
                    rule.TargetBlock = startBlock;
            }

            return rule;
        }

        private void AddActionRuleRights(ActionRule rule, TapestryDesignerSwimlane swimlane)
        {
            if (string.IsNullOrWhiteSpace(swimlane.Roles))
                return;

            foreach (string roleName in swimlane.Roles.Split(','))
            {
                rule.ActionRuleRights.Add(new Entitron.Entity.Persona.ActionRuleRight
                {
                    AppRole = _context.Roles.Single(r => r.ADgroup.ApplicationId == _core.Entitron.AppId && r.Name == roleName),
                    Executable = true
                });
            }
        }

        //private WFitem getItem(TapestryDesignerWorkflowRule workflowRule, int itemType, Func<WFitem, bool> select)
        //{
        //    return
        //        itemType == 0
        //        ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && select(i))
        //        : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && select(i));
        //}
    }

    public class ConnectionTargetSource
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }
}
