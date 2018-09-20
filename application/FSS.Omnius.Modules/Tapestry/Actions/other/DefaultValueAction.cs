using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    public class DefaultValueAction : Action
    {
        public override int Id => 179;

        public override string[] InputVar => new string[] { "VariableName", "Value" };

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override string Name => "Default value";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string varName = (string)vars["VariableName"];
            outputVars["Result"] = (vars.ContainsKey(varName) && vars[varName] != null)
                ? vars[varName]
                : vars["Value"];
        }
    }
}
