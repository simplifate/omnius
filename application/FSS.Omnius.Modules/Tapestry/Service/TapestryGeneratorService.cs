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

        private List<TapestryDesignerConnection> todoConnections;
        Dictionary<WFitem, Block> BlockMapping;
        List<ConnectionTargetSource> splitItems;
        List<ConnectionTargetSource> joinItems;

        public void GenerateTapestry(CORE.CORE core)
        {
            _core = core;
            _context = core.Entitron.GetStaticTables();

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

            // child meta block
            foreach (TapestryDesignerMetablock childMetaBlock in block.Metablocks)
            {
                WorkFlow wf = saveMetaBlock(childMetaBlock);
                wf.Parent = resultWF;
            }

            // child block
            foreach (TapestryDesignerBlock childBlock in block.Blocks)
            {
                saveBlock(childBlock, resultWF);
            }

            // DONE :)
            return resultWF;
        }

        private void saveBlock(TapestryDesignerBlock block, WorkFlow wf)
        {
            // block
            Block resultBlock = new Block
            {
                Name = block.Name,
                ModelName = block.AssociatedTableName,
                WorkFlow = wf
            };
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
            todoConnections = new List<TapestryDesignerConnection>();
            BlockMapping = new Dictionary<WFitem, Block>();
            
            splitItems = workflowRule.Connections.GroupBy(c => new { Type = c.SourceType, Id = c.Source }).Where(c => c.Count() > 1).Select(c => new ConnectionTargetSource { Id = c.Key.Id, Type = c.Key.Type }).ToList();
            joinItems = workflowRule.Connections.GroupBy(c => new { Type = c.TargetType, Id = c.Target }).Where(c => c.Count() > 1).Select(c => new ConnectionTargetSource { Id = c.Key.Id, Type = c.Key.Type }).ToList();

            // get begin
            WFitem item = _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.TypeClass == "circle-single");
            ActionRule actionRule = new ActionRule
            {
                Actor = _context.Actors.Single(a => a.Name == "Manual"),
                Name = workflowRule.Name,
                SourceBlock = block
            };
            TapestryDesignerConnection nextConnection = new TapestryDesignerConnection { Target = item.Id, TargetType = item.GetTypeId() };
            
            while(nextConnection != null)
            {
                // create action
                var result = createAction(nextConnection, actionRule, workflowRule, wf);

                // action rule
                actionRule = result.Item2;

                // next connection || todo connection
                if (result.Item1 == null)
                {
                    // TODO: target block

                    // next connection
                    nextConnection = todoConnections.FirstOrDefault();
                    if (nextConnection != null)
                    {
                        todoConnections.RemoveAt(0);
                        actionRule = null;
                    }
                }
                else
                    nextConnection = result.Item1;
            }
        }

        private Tuple<TapestryDesignerConnection, ActionRule> createAction(TapestryDesignerConnection connection, ActionRule actionRule, TapestryDesignerWorkflowRule workflowRule, WorkFlow wf)
        {
            // INIT
            WFitem target =
                connection.TargetType == 0
                ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Target)
                : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Target);
            
            // starting todo connection
            if (actionRule == null)
            {
                WFitem source =
                    connection.TargetType == 0
                    ? (WFitem)_context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Source)
                    : _context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == connection.Source);
                actionRule = new ActionRule
                {
                    Actor = _context.Actors.Single(a => a.Name == "Auto"),
                    Name = workflowRule.Name,
                    SourceBlock = BlockMapping[source]
                };
                // actionRuleRights
                foreach (string roleName in target.ParentSwimlane.Roles.Split(','))
                {
                    actionRule.ActionRuleRights.Add(new Entitron.Entity.Persona.ActionRuleRight
                    {
                        AppRole = _context.Roles.Single(r => r.Name == roleName),
                        Executable = true
                    });
                }
            }

            //
            TapestryDesignerConnection nextConnection = null;
            ActionRule nextAR = actionRule;

            // split
            if (splitItems.Any(i => i.Id == target.Id && i.Type == target.GetTypeId()))
            {
                // new block
                Block newBlock;
                if (BlockMapping.ContainsKey(target))
                {
                    newBlock = BlockMapping[target];
                }
                else
                {
                    newBlock = new Block
                    {
                        IsVirtual = true,
                        WorkFlow = wf
                    };
                    BlockMapping.Add(target, newBlock);
                }
                // action rules
                if (actionRule != null)
                    actionRule.TargetBlock = newBlock;
                nextAR = new ActionRule
                {
                    Actor = _context.Actors.Single(a => a.Name == "Auto"),
                    Name = workflowRule.Name,
                    SourceBlock = newBlock
                };
                // actionRuleRights
                foreach(string roleName in target.ParentSwimlane.Roles.Split(','))
                {
                    nextAR.ActionRuleRights.Add(new Entitron.Entity.Persona.ActionRuleRight
                    {
                        AppRole = _context.Roles.Single(r => r.Name == roleName),
                        Executable = true
                    });
                }

                // connections
                var connections = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).ToList();
                for(int i = 0; i < connections.Count; i++)
                {
                    if (i == 0)
                        nextConnection = connections[i];
                    else
                        todoConnections.Add(connections[i]);
                }
            }
            // join
            else if (joinItems.Any(i => i.Id == target.Id && i.Type == target.GetTypeId()))
            {
                // new block
                Block newBlock;
                if (BlockMapping.ContainsKey(target))
                {
                    newBlock = BlockMapping[target];
                }
                else
                {
                    newBlock = new Block
                    {
                        IsVirtual = true,
                        WorkFlow = wf
                    };
                    BlockMapping.Add(target, newBlock);
                }

                // action rules
                actionRule.TargetBlock = newBlock;
                actionRule = new ActionRule
                {
                    Actor = _context.Actors.Single(a => a.Name == "Auto"),
                    Name = actionRule.Name,
                    SourceBlock = newBlock
                };
                nextAR = actionRule;
                // actionRuleRights
                foreach (string roleName in target.ParentSwimlane.Roles.Split(','))
                {
                    nextAR.ActionRuleRights.Add(new Entitron.Entity.Persona.ActionRuleRight
                    {
                        AppRole = _context.Roles.Single(r => r.Name == roleName),
                        Executable = true
                    });
                }

                // connection
                nextConnection = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).SingleOrDefault();
            }
            // classic action
            else
            {
                // connections
                nextConnection = workflowRule.Connections.Where(c => c.SourceType == target.GetTypeId() && c.Source == target.Id).SingleOrDefault();
            }


            // create
            if (target.GetTypeId() == 0)
            {
                // TODO: action
                ActionRule_Action result = new ActionRule_Action
                {
                    ActionId = 1, //Action.getByName((target as TapestryDesignerWorkflowItem).Label), TODO: 
                    Order = actionRule.ActionRule_Actions.Any() ? actionRule.ActionRule_Actions.Max(aar => aar.Order) + 1 : 1
                };
                actionRule.ActionRule_Actions.Add(result);
            }
            else
            {
                // TODO: symbols
            }

            return new Tuple<TapestryDesignerConnection, ActionRule>(nextConnection, nextAR);
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
