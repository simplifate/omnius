using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ObjectToJTokenAction : Action
    {
        public override int Id => 2019;
        public override int? ReverseActionId => null;
        public override string[] InputVar => new string[] { "Object" };
        public override string Name => "Object to JToken";
        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            object o = vars.ContainsKey("Object") ? vars["Object"] : null;
            outputVars["Result"] = o == null ? (JToken)(new JObject()) : JToken.FromObject(o);
        }
    }
}
