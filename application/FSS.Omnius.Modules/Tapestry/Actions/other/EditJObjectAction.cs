using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class EditJObjectAction : Action
    {
        public override int Id
        {
            get
            {
                return 181111;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "v$JObject", "PropertyName[index]", "Value[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Edit JObject";
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
            JObject jObject = (JObject)vars["JObject"];
            int propertiesCount = vars.Keys.Where(k => k.StartsWith("PropertyName[") && k.EndsWith("]")).Count();
            int valuesCount = vars.Keys.Where(k => k.StartsWith("Value[") && k.EndsWith("]")).Count();
            if (propertiesCount != valuesCount)
                throw new Exception("Values count differs from properties count!");

            for (int i = 0; i < propertiesCount; i++)
            {
                string propertyName = (string)vars[$"PropertyName[{i}]"];
                var value = vars[$"Value[{i}]"];
                if (jObject.Property(propertyName) != null) {
                    jObject.Property(propertyName).Value = new JValue(value);
                }
                else {
                    jObject.Add(propertyName, new JValue(value));
                }
            }

            outputVars["Result"] = jObject;
        }
    }
}
