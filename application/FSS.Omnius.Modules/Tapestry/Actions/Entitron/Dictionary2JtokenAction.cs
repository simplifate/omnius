using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class Dictionary2JtokenAction : Action
    {
        public override int Id => 10147;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] {"Dictionary"};

        public override string Name => "Dictionary To Jtoken";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            var InputDictionary = vars["Dictionary"];
            outputVars["Result"] = JToken.FromObject(InputDictionary);
        }
    }
}
