using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class JsonStringifyAction : Action
    {
        public override int Id
        {
            get
            {
                return 3003;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "JToken", "?PrettyPrint" };
            }
        }

        public override string Name
        {
            get
            {
                return "Json Stringify";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var jToken = (JToken)vars["JToken"];
            bool prettyPrint = vars.ContainsKey("PrettyPrint") ? (bool)vars["PrettyPrint"] : true;

            if (prettyPrint)
            {
                vars["Result"] = jToken.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                vars["Result"] = jToken.ToString(Newtonsoft.Json.Formatting.None);
            }
        }
    }
}
