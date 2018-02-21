using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class ExponentiationAction : Action
    {
        public override int Id => 4050;

        public override string[] InputVar => new string[] { "Base", "Exponent" };

        public override string Name => "Math: Exponentiation";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var operandA = Convert.ToDouble(vars["Base"]);
            var operandB = Convert.ToDouble(vars["Exponent"]);    
               
            outputVars["Result"] = System.Math.Pow(operandA, operandB);
        }
    }
}
