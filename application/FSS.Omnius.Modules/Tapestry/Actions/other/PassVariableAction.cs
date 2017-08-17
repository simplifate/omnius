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
            /*var registry = new Dictionary<string, object>();
            if (vars.ContainsKey("CrossBlockRegistry")) {
                registry = (Dictionary<string, object>)vars["CrossBlockRegistry"];
            }*/
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            if (core.CrossBlockRegistry.ContainsKey((string)vars["Key"])) {
                if (vars["Value"] == null || (vars["Value"] is string && string.IsNullOrEmpty((string)vars["Value"]))) {
                    core.CrossBlockRegistry.Remove((string)vars["Key"]);
                }
                else {
                    core.CrossBlockRegistry[(string)vars["Key"]] = vars["Value"];
                }
            }
            else {
                core.CrossBlockRegistry.Add((string)vars["Key"], vars["Value"]);
            }
            
            outputVars["Result"] = core.CrossBlockRegistry;
        }
    }
}
