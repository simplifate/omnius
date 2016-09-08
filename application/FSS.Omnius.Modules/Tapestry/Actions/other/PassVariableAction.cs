using FSS.Omnius.Modules.CORE;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class PassVariableAction : Action
    {
        public override int Id
        {
            get
            {
                return 184;
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
                return new string[] { "Key", "Value" };
            }
        }

        public override string Name
        {
            get
            {
                return "Pass variable";
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
            var registry = new Dictionary<string, object>();
            if (vars.ContainsKey("CrossBlockRegistry")) {
                registry = (Dictionary<string, object>)vars["CrossBlockRegistry"];
            }
            registry.Add((string)vars["Key"], vars["Value"]);
            outputVars["Result"] = registry;
        }
    }
}
