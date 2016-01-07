using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class Action_ActionBlock
    {
        public abstract int ActionId { get; set; }
        public abstract int Order { get; set; }
        public abstract string InputVariablesMapping { get; set; }
        public abstract string OutputVariablesMapping { get; set; }

        public Dictionary<string, object> getInputVariables(Dictionary<string, object> tempVars)
        {
            KeyValueString mapping = new KeyValueString(InputVariablesMapping);
            mapping.Resolve(tempVars);
            mapping.result.AddRange(tempVars);
            return mapping.result;
        }
        public void RemapOutputVariables(Dictionary<string, object> actionVars)
        {
            KeyValueString mapping = new KeyValueString(OutputVariablesMapping);
            mapping.ChangeKeysInInput(actionVars);
        }
    }
}
