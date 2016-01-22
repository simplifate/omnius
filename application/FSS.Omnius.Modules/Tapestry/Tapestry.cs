using System;
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

namespace FSS.Omnius.Modules.Tapestry
{
    public class Tapestry : RunableModule
    {
        private CORE.CORE _CORE;
        private ActionResultCollection _results;
        private Page _page;

        public Tapestry(CORE.CORE core)
        {
            Name = "Tapestry";

            _CORE = core;
            _results = new ActionResultCollection();
            _page = null;
        }

        public override void run(User user, string url, NameValueCollection fc) // url = ApplicationId/ActionRuleId/ModelId
        {
            // init
            string AppName;
            int ActionRuleId, modelId;
            splitUrl(url, out AppName, out ActionRuleId, out modelId);

            run(user, AppName, ActionRuleId, modelId, fc);
        }
        public Block run(User user, string AppName, int ActionRuleId, int modelId, NameValueCollection fc)
        {
            // init action
            _results.outputData.Add("__CORE__", _CORE);
            _CORE.Entitron.AppName = AppName;
            _CORE.User = user;
            
            // get actionRule
            ActionRule actionRule = null;
            ActionRule nextRule = GetActionRule(ActionRuleId, _results, modelId);

            // get inputs
            string[] keys = fc.AllKeys;
            foreach(AttributeRule ar in nextRule.SourceBlock.AttributeRules)
            {
                if(keys.Contains(ar.InputName))
                    _results.outputData.Add(ar.AttributeName, Convertor.convert(ar.AttributeDataType, fc[ar.InputName]));
            }

            List<ActionRule> prevActionRules= new List<ActionRule>();
            // run all auto Action
            while (nextRule != null)
            {
                actionRule = nextRule;
                actionRule.Run(_results);

                if (_results.types.Any(x => x == ActionResultType.Error))
                {
                    prevActionRules.Reverse();
                    foreach (ActionRule actRule  in prevActionRules)
                    {
                        actRule.ReverseRun(_results);
                    }
                    break;
                }

                nextRule = GetAutoActionRule(actionRule.TargetBlock, _results);
                prevActionRules.Add(nextRule);
            }

            // target Block
            return actionRule.TargetBlock;
        }

        public override string GetHtmlOutput()
        {
            throw new NotImplementedException();
        }
        public override string GetJsonOutput()
        {
            throw new NotImplementedException();
        }
        public override string GetMailOutput()
        {
            throw new NotImplementedException();
        }

        private void splitUrl(string url, out string ApplicationId, out int ActionRuleId, out int modelId)
        {
            string[] ids = url.Split('/');
            if (ids.Count() != 3)
                throw new ArgumentException("Tapestry needs ActionRuleId and modelId");

            ApplicationId = ids[0];
            ActionRuleId = Convert.ToInt32(ids[1]);
            modelId = Convert.ToInt32(ids[2]);
        }
        
        private ActionRule GetActionRule(int ActionRuleId, ActionResultCollection results, int modelId)
        {
            if (!_CORE.User.canUseAction(ActionRuleId, _CORE.Entitron.GetStaticTables()))
                throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", ActionRuleId));

            ActionRule rule = _CORE.Entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId);

            if (modelId > 0)
                results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(rule.SourceBlock.ModelName, modelId);

            rule.PreRun(results);
            if (!rule.CanRun(results.outputData))
                throw new UnauthorizedAccessException(string.Format("Cannot pass conditions: rule[{0}]", ActionRuleId));

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
