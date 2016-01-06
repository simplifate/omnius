using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class ActionRule_Action
    {
        public Dictionary<string, object> getInputVariables(Dictionary<string, object> tempVars)
        {
            return getInputVariables(InputVariablesMapping, tempVars);
        }
        public void RemapOutputVariables(Dictionary<string, object> actionVars)
        {
            RemapOutputVariables(OutputVariablesMapping, actionVars);
        }

        public static Dictionary<string, object> getInputVariables(string InputVariablesMapping, Dictionary<string, object> tempVars)
        {
            KeyValueString mapping = new KeyValueString(InputVariablesMapping);
            mapping.Resolve(tempVars);
            return mapping.result;
        }
        public static void RemapOutputVariables(string OutputVariablesMapping, Dictionary<string, object> actionVars)
        {
            KeyValueString mapping = new KeyValueString(OutputVariablesMapping);
            mapping.ChangeKeysInInput(actionVars);
        }
    }
}
