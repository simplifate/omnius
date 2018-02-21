using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetHostUrlAction : Action
    {
        public override int Id => 1055;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { };

        public override string Name => "Get Host URL";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            string hostname = TapestryUtils.GetServerHostName();
            outputVars["Result"] = $"{hostname}";
        }
    }
}
