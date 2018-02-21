using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class InitToDatetime : Action
    {
        public override int Id => 950;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Int" };

        public override string Name => "Int To Datetime";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            int timestamp = Convert.ToInt32(vars["Int"]);

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            outputVars["Result"] = dateTime.AddSeconds(timestamp).ToLocalTime();
        }
    }
}
