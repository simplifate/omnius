using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Modules.Tapestry
{
    public class Tapestry : RunableModule
    {
        private CORE.CORE _CORE;
        private DBItem _model;
        private int _PageId;

        public Tapestry(CORE.CORE core)
        {
            Name = "Tapestry";

            _CORE = core;
            _model = null;
            _PageId = -1;
        }

        public override void run(string url) // url = ApplicationId/ActionRuleId/ModelId
        {
            // init
            int ApplicationId, ActionRuleId, modelId;
            splitUrl(url, out ApplicationId, out ActionRuleId, out modelId);

            run(ApplicationId, ActionRuleId, modelId);
        }
        public void run(int ApplicationId, int ActionRuleId, int modelId)
        {
            // confirm rights
            if (!_CORE.Persona.UserCanExecuteActionRule(ActionRuleId))
                throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", ActionRuleId));

            // init - get actionRule
            ActionResultCollection results = new ActionResultCollection();
            ActionRule actionRule = _CORE.Entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId);// confirm conditions

            // get model
            _model = _CORE.Entitron.GetDynamicTable(actionRule.SourceBlock.ModelName).Select().where(c => c.column("Id").Equal(modelId)).First();
            results.outputData.Add("__MODEL__", _model);

            // preRun & conditions
            results.Join = actionRule.PreRun();
            if (!actionRule.CanRun(results.outputData))
                throw new NotAllowedExcetption();
            
            // execute
            results.Join = actionRule.MainRun(results.outputData);

            // execute auto function
            Block finalBlock = actionRule.TargetBlock;
            foreach(ActionRule ar in finalBlock.SourceTo_ActionRoles.Where(ar => ar.Actor.Name == "Auto"))
            {
                ActionResultCollection tempResults = ar.PreRun(results.outputData);
                if (ar.CanRun(results.outputData))
                {
                    results.Join = tempResults;
                    results.Join = ar.MainRun(results.outputData);
                    break;
                }
            }
            
            // get model, pageId for Mozaic
            _PageId = actionRule.TargetBlock.MozaicPageId ?? -1;
        }
        public override string GetHtmlOutput()
        {
            if (_PageId == -1)
                throw new Exception("Must execute 'run' before get html");

            return _CORE.Mozaic.Render(_PageId, _model);
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
    }
}
