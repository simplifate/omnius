using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class PreBlockAction
    {
        public Dictionary<string, object> getInputVariables(Dictionary<string, object> tempVars)
        {
            return ActionRule_Action.getInputVariables(InputVariablesMapping, tempVars);
        }
        public void RemapOutputVariables(Dictionary<string, object> actionVars)
        {
            ActionRule_Action.RemapOutputVariables(OutputVariablesMapping, actionVars);
        }
    }
}
