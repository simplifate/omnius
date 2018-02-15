using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    class ListToStringAction : Action
    {
        public override int Id => 3015;

        public override string[] InputVar => new string[] { "List", "?Separator" };

        public override string Name => "List to string";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var List = (List<object>)vars["List"];
            var Separator = vars.ContainsKey("Separator") ? (string)vars["Separator"] : ",";
            
            outputVars["Result"] = string.Join(Separator, List.ToArray());
        }
    }
}
