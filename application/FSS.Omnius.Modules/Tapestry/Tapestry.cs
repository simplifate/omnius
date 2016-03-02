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
using FSS.Omnius.Modules.Entitron.Entity;
using System.Data;

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
            //string AppName, buttonId;
            //int blockId, modelId;
            //splitUrl(url, out AppName, out blockId, out buttonId, out modelId);

            //run(user, AppName, blockId, buttonId, modelId, fc);
        }
        public Block run(User user, string AppName, Block block, string buttonId, int modelId, NameValueCollection fc)
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

        //private void splitUrl(string url, out string ApplicationId, out int ActionRuleId, out int modelId)
        //{
        //    string[] ids = url.Split('/');
        //    if (ids.Count() != 3)
        //        throw new ArgumentException("Tapestry needs ActionRuleId and modelId");

        //    ApplicationId = ids[0];
        //    ActionRuleId = Convert.ToInt32(ids[1]);
        //    modelId = Convert.ToInt32(ids[2]);
        //}
        
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
