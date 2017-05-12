using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public interface IActionRule_Action
    {
        int ActionRuleId { get; set; }
        ActionRule ActionRule { get; set; }
        int ActionId { get; set; }
        int Order { get; set; }
        string InputVariablesMapping { get; set; }
        string OutputVariablesMapping { get; set; }
    }

    public static class IActionRule_Action_Extension
    {
        public static Dictionary<string, object> getInputVariables(this IActionRule_Action actionRule_action, Dictionary<string, object> tempVars)
        {
            KeyValueString mapping = new KeyValueString(actionRule_action.InputVariablesMapping);
            mapping.Resolve(tempVars);
            mapping.result.AddRange(tempVars);
            return mapping.result;
        }
        public static void RemapOutputVariables(this IActionRule_Action actionRule_action, Dictionary<string, object> actionVars)
        {
            KeyValueString mapping = new KeyValueString(actionRule_action.OutputVariablesMapping);
            mapping.ChangeKeysInInput(actionVars);
        }
    }
}
