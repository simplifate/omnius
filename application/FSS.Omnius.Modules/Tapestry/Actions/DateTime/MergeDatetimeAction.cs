using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class MergeDatetime : Action
    {
        public override int Id => 194;

        public override string[] InputVar => new string[] { "Date", "Time" };

        public override string Name => "Merge Date and Time";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DateTime date = (DateTime)vars["Date"];
            DateTime time = (DateTime)vars["Time"];

            DateTime result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

            outputVars["Result"] = result;
        }
    }
}
