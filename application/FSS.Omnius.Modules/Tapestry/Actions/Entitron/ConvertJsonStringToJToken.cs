using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class ConvertJsonStringToJToken
    {
        public class ConvertJsonStringToJTokenAction : Action
        {
            public override int Id => 5021;

            public override string[] InputVar => new string[] { "JsonString" };

            public override string Name => "ConvertJsonStringToJToken";

            public override string[] OutputVar => new string[] { "Result"};

            public override int? ReverseActionId => null;

            public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
            {
                outputVars["Result"] =  JToken.Parse(vars["JsonString"].ToString());
            }
        }
    }
}
