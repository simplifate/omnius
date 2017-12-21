using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [MathRepository]
    class ExponentiationAction : Action
    {
        public override int Id
        {
            get
            {
                return 4050;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Base", "Exponent" };
            }
        }

        public override string Name
        {
            get
            {
                return "Math: Exponentiation";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var operandA = Convert.ToDouble(vars["Base"]);
            var operandB = Convert.ToDouble(vars["Exponent"]);    
               
            outputVars["Result"] = Math.Pow(operandA, operandB);
        }
    }
}
