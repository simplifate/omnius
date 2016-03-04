using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public abstract class ActionRuleBase
    {
        public abstract void Run(ActionResult results);

        protected void InnerRun(ActionResult results, IEnumerable<ActionRule_ActionBase> actions)
        {
            foreach (ActionRule_ActionBase actionMap in actions.OrderBy(a => a.Order))
            {
                // namapovaní InputVars
                var remapedParams = actionMap.getInputVariables(results.OutputData);

                // Action
                var result = Action.RunAction(actionMap.ActionId, remapedParams);

                // errory
                if (result.Type == ActionResultType.Error)
                {
                    foreach (
                        ActionRule_ActionBase reverseActionMap in
                            actions.Where(a => a.Order < actionMap.Order).OrderByDescending(a => a.Order))
                    {
                        var action = Action.All[reverseActionMap.ActionId];
                        action.ReverseRun(results.ReverseInputData.Last());
                        results.ReverseInputData.Remove(results.ReverseInputData.Last());
                    }

                    // do not continue
                    results.Join(result);
                    return;
                }

                // namapování OutputVars
                //!! pozor na přepisování promněných !!
                actionMap.RemapOutputVariables(result.OutputData);
                // zpracování výstupů
                results.Join(result);
            }
        }

        protected void ReverseInnerRun(ActionResult results, IEnumerable<ActionRule_ActionBase> actions)
        {
            foreach (ActionRule_ActionBase reverseActionMap in actions.OrderByDescending(a => a.Order))
            {
                var action = Action.All[reverseActionMap.ActionId];
                action.ReverseRun(results.ReverseInputData.Last());
                results.ReverseInputData.Remove(results.ReverseInputData.Last());
            }
        }
    }
}
