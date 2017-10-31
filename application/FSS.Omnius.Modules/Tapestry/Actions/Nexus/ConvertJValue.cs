using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ConvertJValueAction : Action
    {
        public override int Id
        {
            get
            {
                return 205;
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
                return new string[] { "From", "s$Type[String|Integer|Double|Bool]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Convert JValue";
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
            var objectType = vars["Type"].ToString();
            switch (objectType)
            {
                case "Integer":
                    outputVars["Result"] = ((JValue)vars["From"]).ToObject<int>();
                    break;
                case "Double":
                    outputVars["Result"] = ((JValue)vars["From"]).ToObject<double>();
                    break;
                case "Bool":
                    outputVars["Result"] = ((JValue)vars["From"]).ToObject<bool>();
                    break;
                default:
                    outputVars["Result"] = ((JValue)vars["From"]).ToObject<string>();
                    break;
            }                    
        }
    }
}
