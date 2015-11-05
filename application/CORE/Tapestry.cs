using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tapestry
{
    public class Tapestry : CORE.RunableModule
    {
        private CORE.CORE _CORE;
        private Entitron.DBItem _model;
        private int _PageId;

        public Tapestry(CORE.CORE core) : base("Tapestry")
        {
            _CORE = core;
            _model = null;
            _PageId = -1;
        }

        public override void run(string url) // url = ActionRuleId/ModelId
        {
            // init
            int ActionRuleId, modelId;
            splitUrl(url, out ActionRuleId, out modelId);

            // confirm rights
            Persona.Persona persona = (Persona.Persona)_CORE.GetModule("Persona");
            if (!persona.UserCanExecuteActionRule(ActionRuleId))
                throw new UnauthorizedAccessException(string.Format("User cannot execute action rule[{0}]", ActionRuleId));

            // get actionRule, model, pageId
            Entitron.Entitron entitron = (Entitron.Entitron)_CORE.GetModule("Entitron");
            Action actionRule = new Action(entitron.GetStaticTables().ActionRules.SingleOrDefault(ar => ar.Id == ActionRuleId));
            _model = entitron.GetDynamicTable(actionRule.entity.SourceBlock.ModelName).Select().where(c => c.column("Id").Equal(modelId)).First();
            _PageId = actionRule.entity.TargetBlock.MozaicPageId ?? -1;

            // execute
            actionRule.run();
        }
        public override string GetHtmlOutput()
        {
            Mozaic.Mozaic mozaic = (Mozaic.Mozaic)_CORE.GetModule("Mozaic");
            if (_PageId == -1)
                throw new Exception("Must execute 'run' before get html");

            return mozaic.Render(_PageId, _model);
        }
        public override string GetJsonOutput()
        {
            throw new NotImplementedException();
        }
        public override string GetMailOutput()
        {
            throw new NotImplementedException();
        }

        private void splitUrl(string url, out int ActionRuleId, out int modelId)
        {
            string[] ids = url.Split('/');
            if (ids.Count() != 2)
                throw new ArgumentException("Tapestry needs ActionRuleId and modelId");

            ActionRuleId = Convert.ToInt32(ids[0]);
            modelId = Convert.ToInt32(ids[1]);
        }
    }
}
