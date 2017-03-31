using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data.Entity;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry
{
    using CORE;
    using Entitron;
    using Entitron.Entity;
    using Entitron.Entity.Tapestry;
    using Entitron.Entity.Persona;
    using Service;

    public class Tapestry : IModule
    {
        private CORE _CORE;
        private ActionResult _results;

        public Tapestry(CORE core)
        {
            _CORE = core;
            _results = new ActionResult();
        }

        public Tuple<Message, Block, Dictionary<string, object>> run(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId, Dictionary<string, object> blockDependencies = null)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc, deleteId, blockDependencies);
            var CrossBlockRegistry = result.Item1.OutputData.ContainsKey("CrossBlockRegistry")
                ? (Dictionary<string, object>)result.Item1.OutputData["CrossBlockRegistry"] : new Dictionary<string, object>();
            return new Tuple<Message, Block, Dictionary<string, object>>(result.Item1.Message, result.Item2, CrossBlockRegistry);
        }
        public JToken jsonRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId = -1)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc, deleteId);
            JObject output = new JObject();
            foreach (KeyValuePair<string, object> pair in result.Item1.OutputData.Where(d => d.Key.StartsWith("__Result[")))
            {
                int startIndex = pair.Key.IndexOf('[') + 1;
                string key = pair.Key.Substring(startIndex, pair.Key.IndexOf(']', startIndex) - startIndex);
                if (pair.Value is string)
                    output.Add(key, (string)pair.Value);
                else if (pair.Value is bool)
                    output.Add(key, (bool)pair.Value);
                else if (pair.Value is int)
                    output.Add(key, (int)pair.Value);
                else if (pair.Value is double)
                    output.Add(key, (double)pair.Value);
                else
                    output.Add(key, pair.Value != null ? (pair.Value as IToJson).ToJson() : null);
            }
            return output;
        }

        public Tuple<ActionResult, Block> innerRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId, Dictionary<string, object> blockDependencies = null)
        {
            // __CORE__
            // __Result[uicName]__
            // __ModelId__
            // __Model.{TableName}.{columnName}
            // __TableName__

            // init action
            _CORE.User = user;
            if (!_results.OutputData.ContainsKey("__CORE__"))
                _results.OutputData.Add("__CORE__", _CORE);
            if (!string.IsNullOrWhiteSpace(block.ModelName))
            {
                if (!_results.OutputData.ContainsKey("__TableName__"))
                    _results.OutputData.Add("__TableName__", block.ModelName);
                else
                    _results.OutputData["__TableName__"] = block.ModelName;
            }
            if (modelId >= 0)
            {
                if (!_results.OutputData.ContainsKey("__ModelId__"))
                    _results.OutputData.Add("__ModelId__", modelId);
                else
                    _results.OutputData["__ModelId__"] = modelId;
            }
            if (deleteId >= 0)
            {
                if (!_results.OutputData.ContainsKey("__DeleteId__"))
                    _results.OutputData.Add("__DeleteId__", deleteId);
                else
                    _results.OutputData["__DeleteId__"] = deleteId;
            }

            if (blockDependencies != null)
            {
                foreach (KeyValuePair<string, object> dependency in blockDependencies)
                {
                    _results.OutputData.Add("__Dependency_" + dependency.Key + "__", dependency.Value);
                }
            }

            // get actionRule
            ActionRule actionRule = null;
            ActionRule nextRule = GetActionRule(block, _results, buttonId);
            if (nextRule == null)
                return new Tuple<ActionResult, Block>(_results, block);

            // get inputs
            if (fc != null)
            {
                string[] keys = fc.AllKeys;
                foreach (string key in keys)
                {
                    _results.OutputData.Add(key, fc[key]);
                }

                // map inputs
                foreach (ResourceMappingPair pair in block.ResourceMappingPairs)
                {
                    if (pair.relationType == "uiItem__attributeItem")
                    {
                        if (fc.AllKeys.Contains(pair.SourceComponentName))
                            _results.OutputData.Add($"__Model.{pair.TargetTableName}.{pair.TargetColumnName}", fc[pair.SourceComponentName]);
                        for (int panelIndex = 1; fc.AllKeys.Contains($"panelCopy{panelIndex}Marker"); panelIndex++)
                        {
                            if (fc.AllKeys.Contains(pair.SourceComponentName))
                            {
                                _results.OutputData.Add($"__Model.panelCopy{panelIndex}.{pair.TargetTableName}.{pair.TargetColumnName}",
                                    fc[$"panelCopy{panelIndex}_{pair.SourceComponentName}"]);
                            }
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
                    return new Tuple<ActionResult, Block>(_results, actionRule.TargetBlock);
                    //return new Tuple<ActionResult, Block>(_results, Rollback(prevActionRules).SourceBlock);
                }

                nextRule = GetActionRule(actionRule.TargetBlock, _results);
            }

            Block resultBlock = actionRule.TargetBlock;
            // if stops on virtual block
            //if (actionRule.TargetBlock.IsVirtual)
            //{
            //    actionRule = Rollback(prevActionRules);
            //    resultBlock = actionRule.SourceBlock;
            //}

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
            DBEntities context = DBEntities.appInstance(_CORE.Entitron.Application);
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
            PersonaAppRole role = context.Roles.Where(r => r.Users.Any(u => u.UserId == _CORE.User.Id) && r.ActionRuleRights.Any(arr => ARs.Contains(arr.ActionRule))).OrderByDescending(r => r.Priority).FirstOrDefault();
            bool hasAnonnymousAccess = role == null && ARs.Any(ar => !ar.ActionRuleRights.Any());
            if (role == null && !hasAnonnymousAccess)
            {
                results.Message.Errors.Add($"You are not authorized to Block [{block.Name}] with Executor[{buttonId}]");
                return null;
            }

            // check
            var swimlaneARs = hasAnonnymousAccess
                ? ARs.Where(ar => !ar.ActionRuleRights.Any()).ToList()
                : ARs.Where(ar => ar.ActionRuleRights.Any(arr => arr.AppRoleId == role.Id)).ToList();
            ActionRule defaultRule = null;
            foreach (ActionRule rule in swimlaneARs)
            {
                // this is default rule (false branch)
                if (rule.isDefault)
                {
                    defaultRule = rule;
                    continue;
                }

                rule.PreRun(results);
                if (rule.ConditionGroup == null 
                    || GatewayDecisionService.MatchConditionSets(rule.ConditionGroup.ConditionSets.ToList(), results.OutputData))
                    return rule;
            }

            if (defaultRule != null)
                return defaultRule;
            
            // nothing meets condition
            results.Message.Errors.Add($"WF can't continue - block[{block.Name}]");
            return null;
        }
    }
}
