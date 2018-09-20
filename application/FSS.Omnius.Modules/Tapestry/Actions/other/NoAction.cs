using System.Collections.Generic;
using System.Threading;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class NoAction : Action
    {
        public override int Id => 181;

        public override string[] InputVar => new string[] { "waitFor" };

        public override string Name => "No Action";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (vars.ContainsKey("waitFor"))
            {
                Thread.Sleep((int)vars["waitFor"]);
            }
        }
    }
}
