using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CopyVariableAction : Action
    {
        public override int Id => 185;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "From" };

        public override string Name => "Copy variable";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (vars["From"] is string)
            {
                outputVars["Result"] = ((string)vars["From"]).Replace("\\n", "\n");
            }
            else
            {
                outputVars["Result"] = vars["From"];
            }
        }
    }
}
