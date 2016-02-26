using FSS.Omnius.Modules.Entitron.Entity;
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

        public void GenerateTapestry(CORE.CORE core)
        {
            _core = core;
            _context = core.Entitron.GetStaticTables();

            var wfToRemove = _context.WorkFlows.Where(w => w.ApplicationId == core.Entitron.AppId);
            _context.WorkFlows.RemoveRange(wfToRemove);
            _context.SaveChanges();

            WorkFlow wf = saveMetaBlock(_core.Entitron.Application.TapestryDesignerRootMetablock, true);
            
            _context.SaveChanges();
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
                saveBlock(childBlock, resultWF);
            }
            _context.SaveChanges();

            // DONE :)
            return resultWF;
        }

        private void saveBlock(TapestryDesignerBlock block, WorkFlow wf)
        {
            // block
            Block resultBlock = new Block
            {
                Name = block.Name,
                ModelName = block.AssociatedTableName
            };
            wf.Blocks.Add(resultBlock);
            if (block.IsInitial)
                resultBlock.InitForWorkFlow.Add(wf);

            TapestryDesignerBlockCommit commit = block.BlockCommits.OrderBy(bc => bc.Timestamp).LastOrDefault();
            if (commit == null) // no commit
                return;
            // Resources
            foreach (TapestryDesignerResourceRule resourceRule in commit.ResourceRules)
            {
                AttributeRule rule = saveResourceRule(resourceRule);
                resultBlock.AttributeRules.Add(rule);
            }

            // ActionRule
            foreach (TapestryDesignerWorkflowRule workflowRule in commit.WorkflowRules)
            {
                saveWFRule(workflowRule, resultBlock, wf);
            }
        }

        private AttributeRule saveResourceRule(TapestryDesignerResourceRule resourceRule)
        {
            AttributeRule result = new AttributeRule();
            foreach(TapestryDesignerConnection connection in resourceRule.Connections)
            {
                TapestryDesignerResourceItem source = resourceRule.ResourceItems.Single(i => i.Id == connection.Source);
                TapestryDesignerResourceItem target = resourceRule.ResourceItems.Single(i => i.Id == connection.Target);

                if (source.TypeClass == "uiItem" && target.TypeClass == "attributeItem")
                    return new AttributeRule
                    {
                        InputName = source.Label,
                        AttributeName = target.Label
                    };
            }

            return null;
        }

        private void saveWFRule(TapestryDesignerWorkflowRule workflowRule, Block block, WorkFlow wf)
        {
            HashSet<TapestryDesignerConnection> todoConnections = new HashSet<TapestryDesignerConnection>();
            Dictionary< WFitem, Block> BlockMapping = new Dictionary<WFitem, Block>();

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
                    Name = "split block",
                    IsVirtual = true
                };
                wf.Blocks.Add(newBlock);
                WFitem it =
                        splitItem.Key.Type == 0
                        ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == splitItem.Key.Id)
                        : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == splitItem.Key.Id);
                BlockMapping.Add(it, newBlock);
            }
            foreach(var joinItem in joinItems)
            {
                // todo connection
                todoConnections.Add(joinItem.FirstOrDefault());

                // block mapping
                Block newBlock = new Block
                {
                    Name = "join block",
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
            WFitem item = _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.TypeClass == "circle-single");
            if (item == null)
                return;
            createActionRule(workflowRule, block, new TapestryDesignerConnection { Target = item.Id, TargetType = 1 }, BlockMapping, "l");
            
            
            //// ACTIONS ////
            foreach(TapestryDesignerConnection conection in todoConnections)
            {
                WFitem it = conection.GetSource(workflowRule, _context);
                Block thisBlock = BlockMapping[it];
                createActionRule(workflowRule, thisBlock, conection, BlockMapping);
            }
            //while(nextConnection != null)
            //{
            //    // create action
            //    var result = createAction(nextConnection, actionRule, workflowRule, wf);

            //    // action rule
            //    actionRule = result.Item2;

            //    // next connection || todo connection
            //    if (result.Item1 == null)
            //    {
            //        // TODO: target block

            //        // next connection
            //        nextConnection = todoConnections.FirstOrDefault();
            //        if (nextConnection != null)
            //        {
            //            todoConnections.RemoveAt(0);
            //            actionRule = null;
            //        }
            //    }
            //    else
            //        nextConnection = result.Item1;
            //}
        }

        private ActionRule createActionRule(TapestryDesignerWorkflowRule workflowRule, Block startBlock, TapestryDesignerConnection connection,
            Dictionary<WFitem, Block> blockMapping, string init = null)
        {
            string ActorName = (init != null ? "Manual" : "Auto");
            ActionRule rule = new ActionRule
            {
                Actor = _context.Actors.Single(a => a.Name == ActorName),
                Name = (new Random()).Next().ToString(),
                ExecutedBy = init
            };
            startBlock.SourceTo_ActionRules.Add(rule);
            AddActionRuleRights(rule, connection.GetTarget(workflowRule, _context).ParentSwimlane);

            WFitem item = connection.GetSource(workflowRule, _context);
            while (item != null && !blockMapping.ContainsKey(item))
            {
                item = connection.GetTarget(workflowRule, _context);

                // create
                if (connection.TargetType == 0)
                {
                    // TODO: action
                    ActionRule_Action result = new ActionRule_Action
                    {
                        ActionId = 1, //Action.getByName((target as TapestryDesignerWorkflowItem).Label), TODO: 
                        Order = rule.ActionRule_Actions.Any() ? rule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1
                    };
                    rule.ActionRule_Actions.Add(result);
                }
                else
                {
                    // TODO: symbols
                }
                

                // next connection
                connection = workflowRule.Connections.First(c => c.Source == connection.Target && c.SourceType == connection.TargetType);
            }

            //if (item != null && blockMapping.ContainsKey(item))
            //    blockMapping[item].TargetTo_ActionRules.Add(rule);

            return rule;
        }

        //private Tuple<TapestryDesignerConnection, ActionRule> createAction(TapestryDesignerConnection connection, ActionRule actionRule, TapestryDesignerWorkflowRule workflowRule, WorkFlow wf)
        //{
        //    // INIT
        //    WFitem target =
        //        connection.TargetType == 0
        //        ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Target)
        //        : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Target);
            
        //    // starting todo connection
        //    if (actionRule == null)
        //    {
        //        WFitem source =
        //            connection.TargetType == 0
        //            ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Source)
        //            : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Source);
        //        actionRule = new ActionRule
        //        {
        //            Actor = _context.Actors.Single(a => a.Name == "Auto"),
        //            Name = workflowRule.Name
        //        };
        //        BlockMapping[source].SourceTo_ActionRules.Add(actionRule);
        //        // actionRuleRights
        //        AddActionRuleRights(actionRule, target.ParentSwimlane);
        //    }

        //    //
        //    TapestryDesignerConnection nextConnection = null;
        //    ActionRule nextAR = actionRule;

        //    // split
        //    if (splitItems.Any(i => i.Id == target.Id && i.Type == target.GetTypeId()))
        //    {
        //        // new block
        //        Block newBlock;
        //        if (BlockMapping.ContainsKey(target))
        //        {
        //            newBlock = BlockMapping[target];
        //        }
        //        else
        //        {
        //            newBlock = new Block
        //            {
        //                Name = $"Split ActionRule[]",
        //                IsVirtual = true
        //            };
        //            wf.Blocks.Add(newBlock);
        //            BlockMapping.Add(target, newBlock);
        //        }
        //        // action rules
        //        if (actionRule != null)
        //            actionRule.TargetBlock = newBlock;
        //        nextAR = new ActionRule
        //        {
        //            Actor = _context.Actors.Single(a => a.Name == "Auto"),
        //            Name = workflowRule.Name
        //        };
        //        newBlock.SourceTo_ActionRules.Add(nextAR);
        //        // actionRuleRights
        //        AddActionRuleRights(nextAR, target.ParentSwimlane);

        //        // connections
        //        var connections = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).ToList();
        //        for(int i = 0; i < connections.Count; i++)
        //        {
        //            if (i == 0)
        //                nextConnection = connections[i];
        //            else
        //                todoConnections.Add(connections[i]);
        //        }
        //    }
        //    // join
        //    // TODO: 2nd time don't continue - duplicity actions
        //    else if (joinItems.Any(i => i.Id == target.Id && i.Type == target.GetTypeId()))
        //    {
        //        // new block
        //        Block newBlock;
        //        if (BlockMapping.ContainsKey(target))
        //        {
        //            newBlock = BlockMapping[target];
        //        }
        //        else
        //        {
        //            newBlock = new Block
        //            {
        //                Name = $"Join ActionRule[]",
        //                IsVirtual = true
        //            };
        //            wf.Blocks.Add(newBlock);
        //            BlockMapping.Add(target, newBlock);
        //        }

        //        // action rules
        //        actionRule.TargetBlock = newBlock;
        //        actionRule = new ActionRule
        //        {
        //            Actor = _context.Actors.Single(a => a.Name == "Auto"),
        //            Name = actionRule.Name
        //        };
        //        newBlock.SourceTo_ActionRules.Add(actionRule);
        //        nextAR = actionRule;
        //        // actionRuleRights
        //        AddActionRuleRights(nextAR, target.ParentSwimlane);

        //        // connection
        //        nextConnection = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).SingleOrDefault();
        //    }
        //    // classic action
        //    else
        //    {
        //        // connections
        //        nextConnection = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).SingleOrDefault();
        //    }


        //    // create
        //    if (target.GetTypeId() == 0)
        //    {
        //        // TODO: action
        //        ActionRule_Action result = new ActionRule_Action
        //        {
        //            ActionId = 1, //Action.getByName((target as TapestryDesignerWorkflowItem).Label), TODO: 
        //            Order = actionRule.ActionRule_Actions.Any() ? actionRule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1
        //        };
        //        actionRule.ActionRule_Actions.Add(result);
        //    }
        //    else
        //    {
        //        // TODO: symbols
        //    }

        //    return new Tuple<TapestryDesignerConnection, ActionRule>(nextConnection, nextAR);
        //}

        private void AddActionRuleRights(ActionRule rule, TapestryDesignerSwimlane swimlane)
        {
            if (true || string.IsNullOrWhiteSpace(swimlane.Roles))
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
