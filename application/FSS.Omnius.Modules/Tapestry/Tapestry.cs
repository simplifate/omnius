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

        public Tuple<Message, Block, Dictionary<string, object>> run(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId, Dictionary<string, object> blockDependencies = null, Dictionary<string, object> mergeVars = null)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc, deleteId, blockDependencies, mergeVars);
            var CrossBlockRegistry = result.Item1.OutputData.ContainsKey("CrossBlockRegistry")
                ? (Dictionary<string, object>)result.Item1.OutputData["CrossBlockRegistry"] : new Dictionary<string, object>();
            return new Tuple<Message, Block, Dictionary<string, object>>(result.Item1.Message, result.Item2, CrossBlockRegistry);
        }

        public JToken jsonRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId = -1)
        {
            Message message = new Message();
            return jsonRun(user, block, buttonId, modelId, fc, out message, deleteId);
        }

        public JToken jsonRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc, out Message message, int deleteId = -1)
        {
            Tuple<ActionResult, Block> result = innerRun(user, block, buttonId, modelId, fc, deleteId);
            JToken output = new JObject();
            foreach (KeyValuePair<string, object> pair in result.Item1.OutputData.Where(d => d.Key.StartsWith("__Result[")))
            {
                int startIndex = pair.Key.IndexOf('[') + 1;
                string key = pair.Key.Substring(startIndex, pair.Key.IndexOf(']', startIndex) - startIndex);
                if (key.Length == 0)
                {
                    if (pair.Value is Dictionary<string, object>)
                        output = JToken.FromObject(pair.Value);                    
                    else
                        output = pair.Value is JToken ? (JToken)pair.Value : (pair.Value as IToJson).ToJson();
                }
                else {
                    if (pair.Value is string)
                        output[key] = (string)pair.Value;
                    else if (pair.Value is bool)
                        output[key] = (bool)pair.Value;
                    else if (pair.Value is int)
                        output[key] = (int)pair.Value;
                    else if (pair.Value is double)
                        output[key] = (double)pair.Value;
                    else if (pair.Value is Dictionary<string,object>)
                        output[key] = JToken.FromObject(pair.Value);
                    else
                        output[key] = pair.Value != null ? (pair.Value as IToJson).ToJson() : null;
                }
            }
            message = result.Item1.Message;
            return output;
        }

        public Tuple<ActionResult, Block> innerRun(User user, Block block, string buttonId, int modelId, NameValueCollection fc, int deleteId, Dictionary<string, object> blockDependencies = null, Dictionary<string, object> mergeVars = null)
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

            if (mergeVars != null) {
                foreach (KeyValuePair<string, object> var in mergeVars) {
                    if (!_results.OutputData.ContainsKey(var.Key)) {
                        _results.OutputData.Add(var.Key, var.Value);
                    }
                    else {
                        _results.OutputData[var.Key] = var.Value;
                    }
                }
            }

            _results.OutputData.Add("__Button__", buttonId);

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

                if (_results.Type == ActionResultType.Error) {
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
            return GetActionRule(_CORE, block, results, buttonId);
        }

        public static ActionRule GetActionRule(CORE core, Block block, ActionResult results, string buttonId = null)
        {
            DBEntities masterContext = DBEntities.instance;
            DBEntities context = DBEntities.appInstance(core.Application);

            // filter by executor
            if (buttonId != null)
            {
                if (!context.ActionRules.Any(ar => ar.SourceBlockId == block.Id && ar.ExecutedBy == buttonId))
                {
                    results.Message.Errors.Add($"Block [{block.Name}] with Executor[{buttonId}] cannot be found");
                    return null;
                }
            }
            else
            {
                if (!context.ActionRules.Any(ar => ar.SourceBlockId == block.Id && ar.Actor.Name == "Auto"))
                    return null;
            }

            // filter by rights
            List<string> roles = masterContext.Users_Roles.Where(ur => ur.UserId == core.User.Id && ur.ApplicationId == core.Application.Id).Select(ur => ur.RoleName).ToList();
            List<ActionRule> ARs = context.ActionRules.SqlQuery($"SELECT *, ISNULL(appr.Priority, 999) as [Prior] FROM Tapestry_ActionRule ar LEFT JOIN Persona_ActionRuleRights arr ON arr.ActionRuleId = ar.Id Left JOIN Persona_AppRoles appr ON appr.Id = arr.AppRoleId WHERE SourceBlockId = @p0 AND ExecutedBy {(buttonId == null ? "IS NULL" : "= @p1")} AND(arr.AppRoleId IS NULL OR appr.Name IN ({(roles.Any() ? string.Join(", ", roles.Select(s => $"N'{s}'")) : "''")})) ORDER BY Prior", block.Id, buttonId).ToList();
            if (!ARs.Any())
            {
                results.Message.Errors.Add($"You are not authorized to Block [{block.Name}] with Executor[{buttonId}]");
                return null;
            }
            
            // filter by conditions
            ActionRule defaultRule = null;
            foreach (ActionRule rule in ARs)
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
