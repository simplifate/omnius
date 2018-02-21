using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    class JsonStringifyAction : Action
    {
        public override int Id => 3003;

        public override string[] InputVar => new string[] { "JToken", "?b$PrettyPrint" };

        public override string Name => "Json Stringify";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var jToken = (JToken)vars["JToken"];
            bool prettyPrint = vars.ContainsKey("PrettyPrint") ? (bool)vars["PrettyPrint"] : false;

            if (prettyPrint)
            {
                outputVars["Result"] = jToken.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                outputVars["Result"] = jToken.ToString(Newtonsoft.Json.Formatting.None);
            }
        }
    }
}
