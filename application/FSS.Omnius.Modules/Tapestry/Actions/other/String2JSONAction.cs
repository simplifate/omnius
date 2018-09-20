using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    public class String2JSONAction : Action
    {
        public override int Id
        {
            get
            {
                return 211;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "From" };
            }
        }

        public override string Name
        {
            get
            {
                return "String to JSON";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            outputVars["Result"] = JToken.Parse((string)vars["From"]);
        }
    }
}
