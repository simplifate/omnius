using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetAppUrlAction : Action
    {
        public override int Id => 1041;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] {  };

        public override string Name => "Get App URL";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            outputVars["Result"] = $"{hostname}/{appName}";
        }
    }
}
