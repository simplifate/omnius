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
        private HashSet<TapestryDesignerBlock> _allBlocks;

        public TapestryGeneratorService()
        {
            _blockMapping = new Dictionary<int, Block>();
            _allBlocks = new HashSet<TapestryDesignerBlock>();
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
                saveMetaBlock(app.TapestryDesignerRootMetablock, true);
                saveBlocks();
                _context.SaveChanges();
                
                // remove old
                _context.WorkFlows.RemoveRange(app.WorkFlows.Where(w => !w.IsTemp));
                _context.SaveChanges();

                foreach (WorkFlow workflow in app.WorkFlows)
                    workflow.IsTemp = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.DiscardChanges();
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
            foreach (TapestryDesignerMetablock childMetaBlock in block.Metablocks.Where(mb => !mb.IsDeleted))
            {
                WorkFlow wf = saveMetaBlock(childMetaBlock);
                wf.Parent = resultWF;
            }
            _context.SaveChanges();

            // child block
            foreach (TapestryDesignerBlock childBlock in block.Blocks.Where(b => !b.IsDeleted))
            {
                TapestryDesignerBlockCommit commit = childBlock.BlockCommits.OrderByDescending(c => c.Timestamp).FirstOrDefault();

                string modelName;
                if (commit == null)
                    modelName = null;
                else
                {
                    if (!string.IsNullOrEmpty(commit.ModelTableName))
                        modelName = commit.ModelTableName;
                    else if (!string.IsNullOrEmpty(commit.AssociatedTableName))
                        modelName = commit.AssociatedTableName.Split(',').First();
                    else
                        modelName = null;
                }

                Block resultBlock = new Block
                {
                    Name = childBlock.Name.RemoveDiacritics(),
                    DisplayName = childBlock.Name,
                    ModelName = modelName,
                    IsVirtual = false,
                    WorkFlow = resultWF
                };
                resultWF.Blocks.Add(resultBlock);
                if (childBlock.IsInitial)
                    resultBlock.InitForWorkFlow.Add(resultWF);

                _blockMapping.Add(childBlock.Id, resultBlock);
                _allBlocks.Add(childBlock);
            }
            _context.SaveChanges();

            // DONE :)
            return resultWF;
        }

        private void saveBlocks()
        {
            foreach(TapestryDesignerBlock block in _allBlocks)
            {
                try
                {
                    saveBlockContent(block);
                }
                catch (Exception e)
                {
                    throw new Exception($"block [{block.Name}] - {e.Message}", e);
                }
            }
            _context.SaveChanges();
        }

        private void saveBlockContent(TapestryDesignerBlock block)
        {
            // block
            Block resultBlock = _blockMapping[block.Id];
            var stateColumnMapping = new Dictionary<int, string>();

            TapestryDesignerBlockCommit commit = block.BlockCommits.OrderBy(bc => bc.Timestamp).LastOrDefault();
            if (commit == null) // no commit
                return;
            // Resources
            foreach (TapestryDesignerResourceRule resourceRule in commit.ResourceRules)
            {
                var pair = saveResourceRule(resourceRule, resultBlock.WorkFlow.Application, stateColumnMapping);
                resultBlock.ResourceMappingPairs.Add(pair);
            }

            // ActionRule
            foreach (TapestryDesignerWorkflowRule workflowRule in commit.WorkflowRules)
            {
                saveWFRule(workflowRule, resultBlock, resultBlock.WorkFlow, stateColumnMapping);
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

        private ResourceMappingPair saveResourceRule(TapestryDesignerResourceRule resourceRule, Application app, Dictionary<int, string> stateColumnMapping)
        {
            AttributeRule result = new AttributeRule();
            using (var context = new DBEntities())
            {
                foreach (TapestryDesignerResourceConnection connection in resourceRule.Connections)
                {
                    TapestryDesignerResourceItem source = connection.Source;
                    TapestryDesignerResourceItem target = connection.Target;

                    string targetName = "", targetType = "", dataSourceParams = "";

                    if (source.ActionId == 1023 || source.ActionId == 1024)
                        continue;

                    if (!string.IsNullOrEmpty(target.ComponentName))
                    {
                        var targetPage = target.Page;
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
                                else if (relatedConnection.Source.ActionId == 1024)
                                    dataSourceParams = "superior";
                            }
                        }
                    }
                    if (source.StateId != null && !string.IsNullOrEmpty(target.ColumnName))
                    {
                        stateColumnMapping.Add(source.StateId.Value, target.ColumnName);
                        continue;
                    }
                    return new ResourceMappingPair
                    {
                        Source = source,
                        Target = target,
                        TargetName = targetName,
                        TargetType = targetType,
                        SourceColumnFilter = source.ColumnFilter,
                        DataSourceParams = dataSourceParams,
                        Block = _blockMapping[connection.ResourceRule.ParentBlockCommit.ParentBlock_Id]
                    };
                }
            }
            return null;
        }

        private void saveWFRule(TapestryDesignerWorkflowRule workflowRule, Block block, WorkFlow wf, Dictionary<int, string> stateColumnMapping)
        {
            HashSet<TapestryDesignerWorkflowConnection> todoConnections = new HashSet<TapestryDesignerWorkflowConnection>();
            Dictionary<Block, int> conditionMapping = new Dictionary<Block, int>();
            Dictionary<TapestryDesignerWorkflowItem, Block> BlockMapping = new Dictionary<TapestryDesignerWorkflowItem, Block>();
            HashSet<Block> blockHasRights = new HashSet<Block> { block };

            // create virtual starting items
            TapestryDesignerWorkflowItem virtualBeginItem = new TapestryDesignerWorkflowItem();
            BlockMapping.Add(virtualBeginItem, block);
            foreach (TapestryDesignerWorkflowItem item in _context.TapestryDesignerWorkflowItems.Where(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && (i.TypeClass == "uiItem" || i.SymbolType == "circle-single")))
            {
                TapestryDesignerWorkflowConnection conn = new TapestryDesignerWorkflowConnection
                {
                    Source = virtualBeginItem,
                    Target = item
                };
                
                todoConnections.Add(conn);
            }

            //
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
                if (it.SymbolType == "gateway-x")
                {
                    conditionMapping.Add(newBlock, it.Id);
                }
                // rights
                if (checkBlockHasRights(splitItem))
                    blockHasRights.Add(newBlock);
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
            
            //// ACTIONS ////
            foreach (TapestryDesignerWorkflowConnection conection in todoConnections)
            {
                TapestryDesignerWorkflowItem it = conection.Source;
                Block thisBlock = BlockMapping[it];
                createActionRule(workflowRule, block, thisBlock, conection, BlockMapping, conditionMapping, stateColumnMapping, blockHasRights);
            }
        }

        private ActionRule createActionRule(TapestryDesignerWorkflowRule workflowRule, Block nonVirtualBlock, Block startBlock, TapestryDesignerWorkflowConnection connection,
            Dictionary<TapestryDesignerWorkflowItem, Block> blockMapping, Dictionary<Block, int> conditionMapping, Dictionary<int, string> stateColumnMapping, HashSet<Block> blockHasRights)
        {
            string init = connection.Target.ComponentName;
            if (workflowRule.Name == "INIT" && nonVirtualBlock == startBlock) // initial ActionRule
                init = "INIT";
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
                // branch true
                if (connection.SourceSlot == 0)
                    rule.ItemWithConditionId = conditionMapping[startBlock];

                // branch false
                else
                    rule.isDefault = true;
            }
            startBlock.SourceTo_ActionRules.Add(rule);
            // rights
            if (blockHasRights.Contains(startBlock))
                AddActionRuleRights(rule, connection.Target.ParentSwimlane);

            TapestryDesignerWorkflowItem item = connection.Target;
            TapestryDesignerWorkflowItem prevItem = null;
            while (item != null && (prevItem == null || !blockMapping.ContainsKey(prevItem)))
            {
                switch (item.TypeClass)
                {
                    case "actionItem":
                        string generatedInputVariables = "";
                        if (item.ActionId == 2005) // Send mail
                        {
                            foreach (var relatedConnections in workflowRule.Connections.Where(c => c.TargetId == item.Id))
                            {
                                if (relatedConnections.Source.TypeClass == "templateItem")
                                    generatedInputVariables = ";Template=s$" + relatedConnections.Source.Label;
                            }
                        }
                        ActionRule_Action result = new ActionRule_Action
                        {
                            ActionId = item.ActionId.Value,
                            Order = rule.ActionRule_Actions.Any() ? rule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1,
                            InputVariablesMapping = item.InputVariables + generatedInputVariables,
                            OutputVariablesMapping = item.OutputVariables
                        };
                        rule.ActionRule_Actions.Add(result);
                        break;

                    case "targetItem":
                        rule.TargetBlock = _blockMapping[item.TargetId.Value];
                        break;

                    case "symbol":
                        switch (item.SymbolType)
                        {
                            case "gateway-x":
                                Block splitBlock = blockMapping[item];
                                // if not already in conditionMapping
                                if (!conditionMapping.ContainsKey(splitBlock))
                                    conditionMapping.Add(splitBlock, item.Id);
                                break;
                        }
                        break;
                    case "circle-thick":
                        break;
                    case "attributeItem":
                        break;
                    case "templateItem":
                        break;
                    case "stateItem":
                        if (stateColumnMapping.ContainsKey(item.StateId.Value))
                        {
                            string stateColumn = stateColumnMapping[item.StateId.Value];
                            ActionRule_Action setStateAction = new ActionRule_Action
                            {
                                ActionId = 1029,
                                Order = rule.ActionRule_Actions.Any() ? rule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1,
                                InputVariablesMapping = $"ColumnName=s${stateColumn};StateId=i${item.StateId.Value}",
                                OutputVariablesMapping = ""
                            };
                            rule.ActionRule_Actions.Add(setStateAction);
                        }
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
                    rule.TargetBlock = nonVirtualBlock;
            }

            return rule;
        }

        private bool checkBlockHasRights(IEnumerable<TapestryDesignerWorkflowConnection> connections)
        {
            if (connections.Count() < 2)
                return false;

            TapestryDesignerSwimlane originalSwimlane = connections.First().Target.ParentSwimlane;
            foreach (var connection in connections)
            {
                if (connection.Target.ParentSwimlane != originalSwimlane)
                    return true;
            }

            return false;
        }

        private void AddActionRuleRights(ActionRule rule, TapestryDesignerSwimlane swimlane)
        {
            if (string.IsNullOrWhiteSpace(swimlane.Roles))
                return;

            foreach (string roleName in swimlane.Roles.Split(','))
            {
                rule.ActionRuleRights.Add(new Entitron.Entity.Persona.ActionRuleRight
                {
                    AppRole = _context.Roles.Single(r => r.ApplicationId == _core.Entitron.AppId && r.Name == roleName),
                    Executable = true
                });
            }
        }
    }

    public class ConnectionTargetSource
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }
}
