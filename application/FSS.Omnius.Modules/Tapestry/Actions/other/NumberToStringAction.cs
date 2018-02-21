using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class NumberToStringAction : Action
    {
        public override int Id => 1105;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Number", "?Format" };

        public override string Name => "Number to String";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string format = vars.ContainsKey("Format") ? vars["Format"].ToString() : "g";
            outputVars["Result"] = (Convert.ToDouble(vars["Number"])).ToString(format);
        }
    }
}
