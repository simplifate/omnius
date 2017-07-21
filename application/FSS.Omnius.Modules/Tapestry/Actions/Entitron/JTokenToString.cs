using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class JTokenToString
    {
        public class JTokenToStringAction : Action
        {
            public override int Id
            {
                get
                {
                    return 5022;
                }
            }

            public override string[] InputVar
            {
                get
                {
                    return new string[] { "value" };
                }
            }

            public override string Name
            {
                get
                {
                    return "JToken to String";
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
                outputVars["Result"] = ((JValue)vars["value"]).ToObject<string>(); ;
            }
        }
    }
}
