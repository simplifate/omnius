using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class ActionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempVars"></param>
        public ActionResultCollection MainRun(Dictionary<string, object> tempVars = null)
        {
            tempVars = tempVars ?? new Dictionary<string, object>();
            var aars = ActionRule_Actions.Where(aar => aar.Order > PreFunctionCount).OrderBy(aar => aar.Order);

            return Run(tempVars, aars);
        }

        /// <summary>
        /// Check Conditions
        /// </summary>
        /// <param name="tempVars">temporary variables to check</param>
        /// <returns>are conditions ok?</returns>
        public bool CanRun(Dictionary<string, object> tempVars)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Run function to prepare conditions
        /// </summary>
        /// <param name="tempVars"></param>
        public ActionResultCollection PreRun(Dictionary<string, object> tempVars = null)
        {
            tempVars = tempVars ?? new Dictionary<string, object>();
            var aars = ActionRule_Actions.Where(aar => aar.Order < PreFunctionCount).OrderBy(aar => aar.Order);

            return Run(tempVars, aars);
        }

        static public ActionResultCollection Run(Dictionary<string, object> tempVars, IEnumerable<ActionRule_Action> aars)
        {
            ActionResultCollection results = new ActionResultCollection();

            foreach (ActionRule_Action aar in aars)
            {
                // namapovaní InputVars
                var remapedParams = aar.getInputVariables(tempVars);
                // Action
                ActionResultCollection result = Modules.Tapestry.Action.RunAction(aar.ActionId, remapedParams);
                // namapování OutputVars
                //!! pozor na přepisování promněných !!
                aar.RemapOutputVariables(result.outputData);
                // zpracování výstupů
                results.Join = result;
                
                // errory
                if (result.types.Any(r => r == ActionResultType.Error))
                    // přerušit? inverzní akce?
                    throw new NotImplementedException();
            }

            return results;
        }
    }
}
