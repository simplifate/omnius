using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class JTokenToString
    {
        public class JTokenToStringAction : Action
        {
            public override int Id => 5022;

            public override string[] InputVar => new string[] { "value" };

            public override string Name => "JToken to String";

            public override string[] OutputVar => new string[] { "Result" };

            public override int? ReverseActionId => null;

            public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
            {
                outputVars["Result"] = ((JValue)vars["value"]).ToObject<string>();
            }
        }
    }
}
