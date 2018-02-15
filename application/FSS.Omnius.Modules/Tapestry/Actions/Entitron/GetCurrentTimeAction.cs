using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetCurrentTimeAction : Action
    {
        public override int Id => 1032;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[0];

        public override string Name => "Get current time";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            outputVars["Result"] = DateTime.Now;
        }
    }
}
