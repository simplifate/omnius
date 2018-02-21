using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ConvertJValueAction : Action
    {
        public override int Id => 205;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "From", "s$Type[String|Integer|Double|Bool]" };

        public override string Name => "Convert JValue";

        public override string[] OutputVar => new string[] { "Result" };

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
