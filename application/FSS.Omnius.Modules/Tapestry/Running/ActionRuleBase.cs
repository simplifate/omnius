using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry
{
    public interface IActionRule
    {
        void Run(ActionResult results);
    }

    public static class ActionRule_Extension
    {
        internal static void InnerRun(this IActionRule actionRule, ActionResult results, IEnumerable<IActionRule_Action> actions)
        {
            foreach (IActionRule_Action actionMap in actions.OrderBy(a => a.Order))
            {
                // namapovaní InputVars
                var remapedParams = actionMap.getInputVariables(results.OutputData);
                ActionResult result;

                if(actionMap.ActionId == -1 && actionMap.VirtualAction == "foreach") { // foreach
                    result = Action.RunForeach(remapedParams, actionMap);
                }
                else { // Action
                    result = Action.RunAction(actionMap.ActionId, remapedParams, actionMap);
                }
                
                // errory
                if (result.Type == MessageType.Error)
                {
                    //foreach (
                    //    IActionRule_Action reverseActionMap in
                    //        actions.Where(a => a.Order < actionMap.Order).OrderByDescending(a => a.Order))
                    //{
                    //    var action = Action.All[reverseActionMap.ActionId];
                    //    action.ReverseRun(results.ReverseInputData.Last());
                    //    results.ReverseInputData.Remove(results.ReverseInputData.Last());
                    //}

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

        internal static void ReverseInnerRun(this IActionRule actionRule, ActionResult results, IEnumerable<IActionRule_Action> actions)
        {
            foreach (IActionRule_Action reverseActionMap in actions.OrderByDescending(a => a.Order))
            {
                var action = Action.All[reverseActionMap.ActionId];
                action.ReverseRun(results.GetLastReverseData());
            }
        }
    }
}
