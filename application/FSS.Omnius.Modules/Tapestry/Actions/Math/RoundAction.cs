using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class MathRound : Action
    {
        public override int Id => 4015;

        public override string[] InputVar => new string[] { "Value", "Precision" };

        public override string Name => "Math: Round";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            String zaokrouhleni = vars["Precision"].ToString();
            int x = Convert.ToInt32(zaokrouhleni);
            double y = 0;
            try
            {
                y = Convert.ToDouble(vars["Value"]);
            }
            catch (Exception) { }
            double zaokrouhleno = System.Math.Round(y, x);
            outputVars["Result"] = zaokrouhleno;
        }
    }
}
