using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GetJObjectPropertyAction : Action
    {
        public override int Id
        {
            get
            {
                return 7253;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "JObject", "PropertyName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get JObject Property";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "JValue" };
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
            JObject input = (JObject)vars["JObject"];
            string propertyName = (string)vars["PropertyName"];
            outputVars["JValue"] = input[propertyName];
        }
    }
}
