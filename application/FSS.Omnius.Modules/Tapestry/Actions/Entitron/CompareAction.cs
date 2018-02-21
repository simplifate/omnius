using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CompareAction : Action
    {
        public override int Id => 1001;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[]
            {
                "model",
                "parameter",
                "value"
            };

        public override string Name => "Compare";

        public override string[] OutputVar => new string[] { "comparation" };
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBItem model = (DBItem)vars["model"];
            string parameter = (string)vars["parameter"];
            object value = vars["value"];

            bool areSame = model[parameter] == value;
            outputVars.Add("comparation", areSame);
        }
    }
}
