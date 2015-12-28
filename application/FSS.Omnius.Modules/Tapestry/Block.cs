using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class Block
    {
        public ActionResultCollection RunPreActionRule(Dictionary<string, object> tempVars)
        {
            tempVars = tempVars ?? new Dictionary<string, object>();
            var preBlockActions = PreBlockActions.OrderBy(pba => pba.Order);
            
            ActionResultCollection results = new ActionResultCollection();

            foreach (PreBlockAction Action in preBlockActions)
            {
                // namapovaní InputVars
                var remapedParams = Action.getInputVariables(tempVars);
                // Action
                ActionResultCollection result = Modules.Tapestry.Action.RunAction(Action.ActionId, remapedParams);
                // namapování OutputVars
                //!! pozor na přepisování promněných !!
                Action.RemapOutputVariables(result.outputData);
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
