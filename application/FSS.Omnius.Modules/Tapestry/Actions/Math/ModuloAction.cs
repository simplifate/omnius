using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class Modulo : Action
    {
        public override int Id => 4004;

        public override string[] InputVar => new string[] { "A", "B", "?AsInteger" };

        public override string Name => "Math: Modulo";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var operandA = vars["A"];
            var operandB = vars["B"];
            bool asInt = vars.ContainsKey("AsInteger") ? (bool)vars["AsInteger"] : false;

            if (asInt)
                outputVars["Result"] = Convert.ToInt64(operandA) % Convert.ToInt64(operandB);
            else
                outputVars["Result"] = Convert.ToDouble(operandA) % Convert.ToDouble(operandB);           
        }
    }
}
