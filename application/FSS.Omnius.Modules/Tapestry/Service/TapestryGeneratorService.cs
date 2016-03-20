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

            var wfToRemove = _context.WorkFlows.Where(w => w.ApplicationId == core.Entitron.AppId);
            _context.WorkFlows.RemoveRange(wfToRemove);
            _context.SaveChanges();

            WorkFlow wf = saveMetaBlock(_core.Entitron.Application.TapestryDesignerRootMetablock, true);

            _context.SaveChanges();

            return _blockMapping;
        }

        private WorkFlow saveMetaBlock(TapestryDesignerMetablock block, bool init = false)
        {
            WorkFlow resultWF = new WorkFlow
            {
                ApplicationId = _core.Entitron.AppId,
                Type = init ? _context.WorkFlowTypes.Single(t => t.Name == "Init") : _context.WorkFlowTypes.Single(t => t.Name == "Partial"),
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
                foreach (TapestryDesignerConnection connection in resourceRule.Connections)
                {
                    TapestryDesignerResourceItem source = resourceRule.ResourceItems.Single(i => i.Id == connection.Source);
                    TapestryDesignerResourceItem target = resourceRule.ResourceItems.Single(i => i.Id == connection.Target);

                    string targetName = "", targetType = "", sourceColumnFilter = "";

                    if (!string.IsNullOrEmpty(target.ComponentName))
                    {
                        var component = context.MozaicEditorPages.Find(target.PageId).Components.Single(c => c.Name == target.ComponentName);
                        targetName = component.Name;
                        targetType = component.Type;
                    };
                    if(!string.IsNullOrEmpty(source.ColumnFilter))
                    {
                        var sourceColumnFilterArray = new List<string>();
                        var idList = source.ColumnFilter.Split(',').Select(int.Parse).ToList();
                        var sourceTable = app.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).First().Tables.Single(c => c.Name == source.TableName);
                        foreach (int columnId in idList)
                        {
                            sourceColumnFilterArray.Add(sourceTable.Columns.Where(c => c.Id == columnId).First().Name);
                        }
                        sourceColumnFilter = string.Join(",", sourceColumnFilterArray);
                    }
                    return new ResourceMappingPair
                    {
                        Source = source,
                        Target = target,
                        TargetName = targetName,
                        TargetType = targetType,
                        SourceColumnFilter = sourceColumnFilter
                    };
                }
            }
            return null;
        }

        private void saveWFRule(TapestryDesignerWorkflowRule workflowRule, Block block, WorkFlow wf)
        {
            HashSet<TapestryDesignerConnection> todoConnections = new HashSet<TapestryDesignerConnection>();
            Dictionary<Block, string> conditionMapping = new Dictionary<Block, string>();
            Dictionary<WFitem, Block> BlockMapping = new Dictionary<WFitem, Block>();

            var splitItems = workflowRule.Connections.GroupBy(c => new { Type = c.SourceType, Id = c.Source }).Where(c => c.Count() > 1);
            var joinItems = workflowRule.Connections.GroupBy(c => new { Type = c.TargetType, Id = c.Target }).Where(c => c.Count() > 1);

            foreach (var splitItem in splitItems)
            {
                // todo connection
                foreach (TapestryDesignerConnection connection in splitItem)
                {
                    todoConnections.Add(connection);
                }

                // block mapping
                Block newBlock = new Block
                {
                    Name = $"splitBlock_{block.Name}",
                    DisplayName = $"split block [{block.Name}]",
                    ModelName = block.ModelName,
                    IsVirtual = true
                };
                wf.Blocks.Add(newBlock);
                WFitem it =
                        splitItem.Key.Type == 0
                        ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == splitItem.Key.Id)
                        : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == splitItem.Key.Id);
                BlockMapping.Add(it, newBlock);

                // conditions
                if (it.TypeClass == "gateway-x")
                {
                    conditionMapping.Add(newBlock, (it as TapestryDesignerWorkflowSymbol).Condition);
                }
            }
            foreach (var joinItem in joinItems)
            {
                // todo connection
                todoConnections.Add(joinItem.FirstOrDefault());

                // block mapping
                Block newBlock = new Block
                {
                    Name = $"joinBlock_{block.Name}",
                    DisplayName = $"join block [{block.Name}]",
                    ModelName = block.ModelName,
                    IsVirtual = true
                };
                wf.Blocks.Add(newBlock);

                foreach (var ji in joinItem)
                {
                    WFitem it =
                            ji.SourceType == 0
                            ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == ji.Source)
                            : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == ji.Source);
                    BlockMapping.Add(it, newBlock);
                }
            }

            // begin
            WFitem item = _context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.TypeClass == "uiItem");
            if (item == null)
                return;
            createActionRule(workflowRule, block, new TapestryDesignerConnection { Target = item.Id, TargetType = 0 }, BlockMapping, conditionMapping, (item as TapestryDesignerWorkflowItem).ComponentId);


            //// ACTIONS ////
            foreach (TapestryDesignerConnection conection in todoConnections)
            {
                WFitem it = conection.GetSource(workflowRule, _context);
                Block thisBlock = BlockMapping[it];
                createActionRule(workflowRule, thisBlock, conection, BlockMapping, conditionMapping);
            }
        }

        private ActionRule createActionRule(TapestryDesignerWorkflowRule workflowRule, Block startBlock, TapestryDesignerConnection connection,
            Dictionary<WFitem, Block> blockMapping, Dictionary<Block, string> conditionMapping, string init = null)
        {
            string ActorName = (init != null ? "Manual" : "Auto");
            ActionRule rule = new ActionRule
            {
                Actor = _context.Actors.Single(a => a.Name == ActorName),
                Name = (new Random()).Next().ToString(),
                ExecutedBy = init
            };
            // condition
            if (conditionMapping.ContainsKey(startBlock))
            {
                rule.Condition = connection.SourceSlot == 0 ? conditionMapping[startBlock] : $"!{conditionMapping[startBlock]}";
            }
            startBlock.SourceTo_ActionRules.Add(rule);
            // rights
            AddActionRuleRights(rule, connection.GetTarget(workflowRule, _context).ParentSwimlane);

            WFitem item = connection.GetTarget(workflowRule, _context);
            WFitem prevItem = null;
            while (item != null && (prevItem == null || !blockMapping.ContainsKey(prevItem)))
            {
                // create
                if (connection.TargetType == 0)
                {
                    TapestryDesignerWorkflowItem wfItem = (TapestryDesignerWorkflowItem)item;
                    // action
                    if (wfItem.ActionId != null)
                    {
                        ActionRule_Action result = new ActionRule_Action
                        {
                            ActionId = wfItem.ActionId.Value,
                            Order = rule.ActionRule_Actions.Any() ? rule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1,
                            InputVariablesMapping = wfItem.InputVariables,
                            OutputVariablesMapping = wfItem.OutputVariables
                        };
                        rule.ActionRule_Actions.Add(result);
                    }
                    // target
                    if (wfItem.TargetId != null)
                    {
                        rule.TargetBlock = _blockMapping[wfItem.TargetId.Value];
                    }

                    // TODO: other items
                }
                else
                {
                    TapestryDesignerWorkflowSymbol wfSymbol = (TapestryDesignerWorkflowSymbol)item;
                    // gateway-x
                    if (wfSymbol.TypeClass == "gateway-x")
                    {
                        Block splitBlock = blockMapping[item];
                        // if not already in conditionMapping
                        if (!conditionMapping.ContainsKey(splitBlock))
                            conditionMapping.Add(splitBlock, wfSymbol.Condition);
                    }

                    // TODO: symbols
                }


                // next connection
                connection = workflowRule.Connections.FirstOrDefault(c => c.Source == connection.Target && c.SourceType == connection.TargetType);
                prevItem = item;
                item = connection != null ? connection.GetTarget(workflowRule, _context) : null;
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
