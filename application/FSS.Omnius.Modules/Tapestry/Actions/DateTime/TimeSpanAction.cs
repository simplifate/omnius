using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Time
{
    public class TimeSpanAction : Action
    {
        public override int Id => 801;

        public override string[] InputVar => new string[] { "?From", "?To" }; // pokud není nastaveno, použije se Now

        public override string Name => "Time span";

        public override string[] OutputVar => new string[] { "Result" }; // int - ms

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DateTime from =
                vars.ContainsKey("From")
                ? (DateTime)vars["From"]
                : DateTime.Now;

            DateTime to =
                vars.ContainsKey("To")
                ? (DateTime)vars["To"]
                : DateTime.Now;

            outputVars.Add("Result", (int)(to - from).TotalMilliseconds);
        }
    }
}
