using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    public class Json2DictAction : Action
    {
        public override int Id => 30234;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "JsonObject" };

        public override string[] OutputVar => new string[] { "Result" };

        public override string Name => "JsonToDictionary";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JObject data = (JObject)vars["JsonObject"];
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (JProperty property in data.Properties()) {
                var value = property.Value.ToString();
                var key = property.Name;
                dict.Add(key, value);
            }

            outputVars["Result"] = dict;
            
        }
    }
}
