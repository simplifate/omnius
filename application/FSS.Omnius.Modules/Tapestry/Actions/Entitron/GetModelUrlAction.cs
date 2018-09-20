using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetModelUrlAction : Action
    {
        public override int Id => 1031;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "BlockName", "Id" };

        public override string Name => "Get Model URL";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            COREobject core = COREobject.i;
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            string blockName = (string)vars["BlockName"];
            int modelId = (int)vars["Id"];
            outputVars["Result"] = $"{hostname}/{appName}/{blockName}?modelId={modelId}";
        }
    }
}
