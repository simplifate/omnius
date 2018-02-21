using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class FormatDatetime : Action
    {
        public override int Id => 187;

        public override string[] InputVar => new string[] { "Input", "?Format" };

        public override string Name => "Format DateTime";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DateTime dateTime = (DateTime)vars["Input"];
            string format = vars.ContainsKey("Format") ? (string)vars["Format"] : "o";

            outputVars["Result"] = dateTime.ToLocalTime().ToString(format);
        }
    }
}
