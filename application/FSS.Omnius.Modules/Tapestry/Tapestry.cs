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
            _CORE.Entitron.AppId = ApplicationId;
            Block targetBlock;
            
            // get target Block
            if (ActionRuleId == null)
            {
                targetBlock = _CORE.Entitron.GetStaticTables().Applications.SingleOrDefault(app => app.Id == ApplicationId).WorkFlows.SingleOrDefault(wf => wf.Type.Name == "Init").InitBlock;
            }
            else
            {
                // get actionRule
                ActionRule actionRule = null;
                ActionRule nextRule = GetActionRule(_CORE.Entitron.Application, ActionRuleId.Value, _results, modelId);

                // get inputs
                string[] keys = fc.AllKeys;
                foreach(AttributeRule ar in nextRule.SourceBlock.AttributeRules)
                {
                    if(keys.Contains(ar.InputName))
                        _results.outputData.Add(ar.AttributeName, Convertor.convert(ar.AttributeDataType, fc[ar.InputName]));
                }

                // run all auto Action
                while (nextRule != null)
                {
                    actionRule = nextRule;
                    actionRule.Run(_results);
                    nextRule = GetAutoActionRule(_CORE.Entitron.Application, actionRule.TargetBlock, _results);
                }

                // target Block
                targetBlock = actionRule.TargetBlock;
            }

            // run PreAction & get page
            targetBlock.Run(_results);
            _page = targetBlock.MozaicPage;
        }

        public override string GetHtmlOutput()
        {
            if (_page == null)
                throw new Exception("Must execute 'run' before get html");

            return _page.MasterTemplate.Html;
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
        
        private ActionRule GetActionRule(Application application, int ActionRuleId, ActionResultCollection results, int? modelId = null)
        {
            //if (!_CORE.Persona.UserCanExecuteActionRule(ActionRuleId))
            //    throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", ActionRuleId));

            ActionRule rule = _CORE.Entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId);

            if (modelId != null)
                results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(application, rule.SourceBlock.ModelName, modelId.Value);

            rule.PreRun(results);
            if (!rule.CanRun(results.outputData))
                throw new UnauthorizedAccessException(string.Format("Cannot pass conditions: rule[{0}]", ActionRuleId));

            return rule;
        }
        private ActionRule GetAutoActionRule(Application application, Block block, ActionResultCollection results, int? modelId = null)
        {
            foreach (ActionRule ar in block.SourceTo_ActionRoles.Where(ar => ar.Actor.Name == "Auto" && _CORE.Persona.UserCanExecuteActionRule(ar.Id)))
            {
                if (modelId != null)
                    results.outputData["__MODEL__"] = _CORE.Entitron.GetDynamicItem(application, block.ModelName, modelId.Value);

                ar.PreRun(results);
                if (ar.CanRun(results.outputData))
                    return ar;
            }

            return null;
        }
    }
}
