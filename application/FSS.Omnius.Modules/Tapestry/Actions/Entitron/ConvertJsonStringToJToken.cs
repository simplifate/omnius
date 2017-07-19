using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class ConvertJsonStringToJToken
    {
        public class ConvertJsonStringToJTokenAction : Action
        {
            public override int Id
            {
                get
                {
                    return 5020;
                }
            }

            public override string[] InputVar
            {
                get
                {
                    return new string[] { "JsonString" };
                }
            }

            public override string Name
            {
                get
                {
                    return "ConvertJsonStringToJToken";
                }
            }

            public override string[] OutputVar
            {
                get
                {
                    return new string[] { "Result"};
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
                outputVars["Result"] =  JToken.Parse(vars["JsonString"].ToString());

            }
        }
    }
}
