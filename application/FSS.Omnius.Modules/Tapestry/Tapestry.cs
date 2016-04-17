using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Tapestry
{
    public class Tapestry : RunableModule
    {
        private CORE.CORE _CORE;
        private ActionResult _results;

        public Tapestry(CORE.CORE core)
        {
            Name = "Tapestry";

            _CORE = core;
            _results = new ActionResult();
        }
        
        public Tuple<Message, Block> run(User user, Block block, string buttonId, int modelId, NameValueCollection fc)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc);
            return new Tuple<Message, Block>(result.Item1.Message, result.Item2);
        }
        public JToken jsonRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc);
            JObject output = new JObject();
            foreach(KeyValuePair<string, object> pair in result.Item1.OutputData.Where(d => d.Key.StartsWith("__Result[")))
            {
                int startIndex = pair.Key.IndexOf('[') + 1;
                string key = pair.Key.Substring(startIndex, pair.Key.IndexOf(']', startIndex) - startIndex);
                if (pair.Value is string)
                    output.Add(key, (string)pair.Value);
                else
                    output.Add(key, pair.Value != null ? (pair.Value as IToJson).ToJson() : null);
            }
            return output;
        }
        private Tuple<ActionResult, Block> innerRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc)
        {
            // __CORE__
            // __Result[uicName]__
            // __ModelId__
            // __Model.{TableName}.{columnName}
            // __TableName__

            // init action
            fc = fc ?? new NameValueCollection();
            _CORE.User = user;
            _results.OutputData.Add("__CORE__", _CORE);
            if (!string.IsNullOrWhiteSpace(block.ModelName))
                _results.OutputData.Add("__TableName__", block.ModelName);
            if (modelId >= 0)
                _results.OutputData.Add("__ModelId__", modelId);

            // get actionRule
            ActionRule actionRule = null;
            ActionRule nextRule = GetActionRule(block, _results, buttonId);
            if (nextRule == null)
                return new Tuple<ActionResult, Block>(_results, block);

            // get inputs
            string[] keys = fc.AllKeys;
            foreach (string key in keys)
            {
                _results.OutputData.Add(key, fc[key]);
            }

            // map inputs
            foreach (ResourceMappingPair pair in block.ResourceMappingPairs)
            {
                TapestryDesignerResourceItem source = pair.Source;
                TapestryDesignerResourceItem target = pair.Target;

                if (source.TypeClass == "uiItem" && target.TypeClass == "attributeItem")
                {
                    if (fc.AllKeys.Contains(source.ComponentName))
                        _results.OutputData.Add($"__Model.{target.TableName}.{target.ColumnName}", fc[source.ComponentName]);
                    for (int panelIndex = 1; fc.AllKeys.Contains($"panelCopy{panelIndex}Marker"); panelIndex++)
                    {
                        if (fc.AllKeys.Contains(source.ComponentName))
                        {
                            _results.OutputData.Add($"__Model.panelCopy{panelIndex}.{target.TableName}.{target.ColumnName}",
                                fc[$"panelCopy{panelIndex}_" + source.ComponentName]);
                        }
                    }
                }
            }

            List<ActionRule> prevActionRules = new List<ActionRule>();
            // run all auto Action
            while (nextRule != null)
            {
                actionRule = nextRule;
                prevActionRules.Add(nextRule);
                actionRule.Run(_results);

                if (_results.Type == ActionResultType.Error)
                {
                    return new Tuple<ActionResult, Block>(_results, Rollback(prevActionRules).SourceBlock);
                }

                nextRule = GetActionRule(actionRule.TargetBlock, _results);
            }

            Block resultBlock = actionRule.TargetBlock;
            // if stops on virtual block
            if (actionRule.TargetBlock.IsVirtual)
            {
                actionRule = Rollback(prevActionRules);
                resultBlock = actionRule.SourceBlock;
            }

            // target Block
            return new Tuple<ActionResult, Block>(_results, resultBlock);
        }

        private ActionRule Rollback(List<ActionRule> prevActionRules)
        {
            prevActionRules.Reverse();
            foreach (ActionRule actRule in prevActionRules)
            {
                actRule.ReverseRun(_results);
            }
            return prevActionRules.Last();
        }

        private ActionRule GetActionRule(Block block, ActionResult results, string buttonId = null)
        {
            DBEntities context = _CORE.Entitron.GetStaticTables();
            IQueryable<ActionRule> ARs;
            if (buttonId != null)
            {
                ARs = context.ActionRules.Where(ar => ar.SourceBlockId == block.Id && ar.ExecutedBy == buttonId);
                if (!ARs.Any())
                {
                    results.Message.Errors.Add($"Block [{block.Name}] with Executor[{buttonId}] cannot be found");
                    return null;
                }
            }
            else
            {
                ARs = context.ActionRules.Where(ar => ar.SourceBlockId == block.Id && ar.Actor.Name == "Auto");
                if (!ARs.Any())
                    return null;
            }

            // authorize
            PersonaAppRole role = _CORE.Entitron.GetStaticTables().Roles.Where(r => r.Users.Any(u => u.UserId == _CORE.User.Id) && r.ActionRuleRights.Any(arr => ARs.Contains(arr.ActionRule))).OrderByDescending(r => r.Priority).FirstOrDefault();
            bool hasAnonnymousAccess = role == null && ARs.Any(ar => !ar.ActionRuleRights.Any());
            if (role == null && !hasAnonnymousAccess)
            {
                results.Message.Errors.Add($"You are not authorized to Block [{block.Name}] with Executor[{buttonId}]");
                return null;
            }

            // check
            var swimlaneARs = hasAnonnymousAccess ? ARs.Where(ar => !ar.ActionRuleRights.Any()) : ARs.Where(ar => ar.ActionRuleRights.Any(arr => arr.AppRoleId == role.Id));
            foreach(var rule in swimlaneARs)
            {
                rule.PreRun(results);
                if (rule.CanRun(results.OutputData))
                    return rule;
            }
            
            // nothing meets condition
            results.Message.Errors.Add($"WF can't continue - block[{block.Name}]");
            return null;
        }
    }
}
