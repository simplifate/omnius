using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
   
    
    public class Json2DictAction : Action
    {
        public override int Id
        {
            get
            {
                return 30234;
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
                return new string[] { "JsonObject" };
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get
            {
                return "JsonToDictionary";
            }
        }

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
