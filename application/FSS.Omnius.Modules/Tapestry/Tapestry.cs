using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Entitron;
using FSS.Omnius.Entitron.Entity.CORE;
using FSS.Omnius.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Tapestry
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

            // get actionRule, model, pageId
            ActionRule actionRule = _CORE.Entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId);
            _model = _CORE.Entitron.GetDynamicTable(actionRule.SourceBlock.ModelName).Select().where(c => c.column("Id").Equal(modelId)).First();
            _PageId = actionRule.TargetBlock.MozaicPageId ?? -1;

            // execute
            actionRule.Run();

            Block finalBlock = actionRule.TargetBlock;
            foreach(ActionRule ar in finalBlock.SourceTo_ActionRoles.Where(ar => ar.Actor.Name == "Auto"))
            {
                throw new NotImplementedException();
            }
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
