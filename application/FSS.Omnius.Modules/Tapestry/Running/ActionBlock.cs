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

                // errory
                if (result.types.Any(r => r == ActionResultType.Error))
                {
                    foreach (Action_ActionBlock reverseActionMap in actions.Where(a => a.Order < actionMap.Order).OrderByDescending(a => a.Order))
                    {
                        var action = Action.All[reverseActionMap.ActionId];
                        action.ReverseRun(results.ReverseInputData.Last());
                        results.ReverseInputData.Remove(results.ReverseInputData.Last());
                    }

                    // do not continue
                    return;
                }

                // namapování OutputVars
                //!! pozor na přepisování promněných !!
                actionMap.RemapOutputVariables(result.outputData);
                // zpracování výstupů
                results.Join = result;
            }
        }
    }
}
