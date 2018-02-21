using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Time
{
    public class AddTimeSpanAction : Action
    {
        public override int Id => 802;

        public override string[] InputVar => new string[] { "?StartTime", "TimeSpan" };
        // StartTime - datetime; default now
        // timespan - int ms

        public override string Name => "Add TimeSpan to DateTime";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DateTime from =
                vars.ContainsKey("StartTime")
                ? (DateTime)vars["StartTime"]
                : DateTime.Now;

            if (!vars.ContainsKey("TimeSpan"))
                throw new ArgumentNullException("TimeSpan is empty");

            TimeSpan diff = TimeSpan.FromMilliseconds((int)vars["TimeSpan"]);
            outputVars.Add("Result", from + diff);
        }
    }
}
