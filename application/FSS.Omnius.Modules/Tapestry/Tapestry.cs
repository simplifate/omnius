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

        public override void run(string url, NameValueCollection fc) // url = ApplicationId/ActionRuleId/ModelId
        {
            // init
            int ApplicationId, ActionRuleId, modelId;
            splitUrl(url, out ApplicationId, out ActionRuleId, out modelId);

            run(ApplicationId, ActionRuleId, modelId, fc);
        }
        public void run(int ApplicationId, int? ActionRuleId, int? modelId, NameValueCollection fc)
        {
            // init action
            _results.outputData.Add("__CORE__", _CORE);
            string AppName = "TestApp";

            ActionRule actionRule = 
                ActionRuleId != null
                ? GetActionRule(AppName, ActionRuleId.Value, _results, modelId) 
                : GetAutoActionRule(
                    AppName,
                    _CORE.Entitron.GetStaticTables().Applications.SingleOrDefault(app => app.Id == ApplicationId).WorkFlows.SingleOrDefault(wf => wf.Type.Name == "Init").InitBlock,
                    _results,
                    modelId);

            while (actionRule != null)
            {
                _results.Join = actionRule.MainRun(_results.outputData);
                actionRule = GetAutoActionRule(AppName, actionRule.TargetBlock, _results);
            }

            // get model, page for Mozaic
            actionRule.TargetBlock.RunPreActionRule(_results.outputData);
            _page = actionRule.TargetBlock.MozaicPage;
        }
        public override string GetHtmlOutput()
        {
            if (_page == null)
                throw new Exception("Must execute 'run' before get html");

            return _CORE.Mozaic.Render(_page, null);
        }
        public override string GetJsonOutput()
        {
            throw new NotImplementedException();
        }
        public override string GetMailOutput()
        {
            throw new NotImplementedException();
        }

        private void splitUrl(string url, out int ApplicationId, out int ActionRuleId, out int modelId)
        {
            string[] ids = url.Split('/');
            if (ids.Count() != 3)
                throw new ArgumentException("Tapestry needs ActionRuleId and modelId");

            ApplicationId = Convert.ToInt32(ids[0]);
            ActionRuleId = Convert.ToInt32(ids[1]);
            modelId = Convert.ToInt32(ids[2]);
        }
        
        private ActionRule GetActionRule(string AppName, int ActionRuleId, ActionResultCollection results, int? modelId = null)
        {
            if (!_CORE.Persona.UserCanExecuteActionRule(ActionRuleId))
                throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", ActionRuleId));

            ActionRule rule = _CORE.Entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId);

            if (modelId != null)
                results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(AppName, rule.SourceBlock.ModelName, modelId.Value);

            results.Join = rule.PreRun(results.outputData);
            if (!rule.CanRun(results.outputData))
                throw new UnauthorizedAccessException(string.Format("Cannot pass conditions: rule[{0}]", ActionRuleId));

            return rule;
        }
        private ActionRule GetAutoActionRule(string AppName, Block block, ActionResultCollection results, int? modelId = null)
        {
            foreach (ActionRule ar in block.SourceTo_ActionRoles.Where(ar => ar.Actor.Name == "Auto" && _CORE.Persona.UserCanExecuteActionRule(ar.Id)))
            {
                if (modelId != null)
                    results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(AppName, block.ModelName, modelId ?? -1); // never happened

                ActionResultCollection tempResults = ar.PreRun(results.outputData);
                if (ar.CanRun(results.outputData))
                {
                    results.Join = tempResults;
                    return ar;
                }
            }

            return null;
        }
    }
}
