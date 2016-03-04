﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;
using System.Collections.Specialized;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Data;

namespace FSS.Omnius.Modules.Tapestry
{
    public class Tapestry : RunableModule
    {
        private CORE.CORE _CORE;
        private ActionResultCollection _results;

        public Tapestry(CORE.CORE core)
        {
            Name = "Tapestry";

            _CORE = core;
            _results = new ActionResultCollection();
        }
        
        public Tuple<ActionResultCollection, Block> run(User user, string AppName, Block block, string buttonId, int modelId, NameValueCollection fc)
        {
            // init action
            _results.outputData.Add("__CORE__", _CORE);
            _CORE.Entitron.AppName = AppName;
            _CORE.User = user;

            // get actionRule
            ActionRule actionRule = null;
            ActionRule nextRule = GetActionRule(block, buttonId, _results, modelId);

            // get inputs
            string[] keys = fc.AllKeys;

            List<ActionRule> prevActionRules = new List<ActionRule>();
            // run all auto Action
            while (nextRule != null)
            {
                actionRule = nextRule;
                actionRule.Run(_results);

                if (_results.types.Any(x => x == ActionResultType.Error))
                {
                    return new Tuple<ActionResultCollection, Block>(_results, Rollback(prevActionRules, nextRule).TargetBlock);
                }

                nextRule = GetAutoActionRule(actionRule.TargetBlock, _results);
                prevActionRules.Add(nextRule);
            }

            // if stops on virtual block
            if (actionRule.TargetBlock.IsVirtual)
            {
                actionRule = Rollback(prevActionRules, actionRule);
            }

            // target Block
            return new Tuple<ActionResultCollection, Block>(_results, actionRule.TargetBlock);
        }

        public ActionRule Rollback(List<ActionRule> prevActionRules, ActionRule thisRule)
        {
            prevActionRules.Reverse();
            foreach (ActionRule actRule in prevActionRules)
            {
                actRule.ReverseRun(_results);
            }
            return prevActionRules.Count > 0 ? prevActionRules.Last() : thisRule;
        }

        private ActionRule GetActionRule(Block block, string buttonId, ActionResultCollection results, int modelId)
        {
            ActionRule rule = block.SourceTo_ActionRules.SingleOrDefault(ar => ar.ExecutedBy == buttonId);
            if (rule == null)
                throw new MissingMethodException($"Block [{block.Name}] with Executor[{buttonId}] cannot be found");

            if (false && !_CORE.User.canUseAction(rule.Id, _CORE.Entitron.GetStaticTables()))
                throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", rule.Id));

            if (modelId > 0)
                results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(rule.SourceBlock.ModelName, modelId);

            rule.PreRun(results);
            if (false && !rule.CanRun(results.outputData))
                throw new UnauthorizedAccessException(string.Format("Cannot pass conditions: rule[{0}]", rule.Id));

            return rule;
        }
        private ActionRule GetAutoActionRule(Block block, ActionResultCollection results, int? modelId = null)
        {
            foreach (ActionRule ar in block.SourceTo_ActionRules.Where(ar => ar.Actor.Name == "Auto" && _CORE.User.canUseAction(ar.Id, _CORE.Entitron.GetStaticTables())))
            {
                if (modelId != null)
                    results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(block.ModelName, modelId.Value);

                ar.PreRun(results);
                if (ar.CanRun(results.outputData))
                    return ar;
            }

            return null;
        }
    }
}
