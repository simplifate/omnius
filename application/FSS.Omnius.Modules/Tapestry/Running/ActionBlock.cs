using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class ActionBlock
    {
        public abstract void Run(ActionResultCollection results);

        protected void InnerRun(ActionResultCollection results, IEnumerable<Action_ActionBlock> actions)
        {
            foreach (Action_ActionBlock actionMap in actions.OrderBy(a => a.Order))
            {
                // namapovaní InputVars
                var remapedParams = actionMap.getInputVariables(results.outputData);
                // Action
                var result = Action.RunAction(actionMap.ActionId, remapedParams);
                // namapování OutputVars
                //!! pozor na přepisování promněných !!
                actionMap.RemapOutputVariables(result.outputData);
                // zpracování výstupů
                results.Join = result;

                // errory
                if (result.types.Any(r => r == ActionResultType.Error))
                    // přerušit? inverzní akce?
                    throw new NotImplementedException();
            }
        }
    }
}
