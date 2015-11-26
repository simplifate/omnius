using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Entity.Tapestry
{
    public partial class ActionRule
    {
        public void Run()
        {
            Dictionary<string, object> tempVars = new Dictionary<string, object>();
            List<string> messages = new List<string>();

            foreach (ActionRule_Action aar in ActionRule_Actions.OrderBy(aar => aar.Order))
            {
                // namapovaní InputVars
                var remapedParams = aar.getInputVariables(tempVars);
                // Action
                ActionResult ar = Modules.Tapestry.Action.RunAction(aar.ActionId, remapedParams);
                // zpracování výstupů
                if (ar.type != ActionResultType.Success)
                {
                    messages.Add(ar.Message);
                    // errory
                    if (ar.type == ActionResultType.Error)
                        // přerušit? inverzní akce?
                        throw new NotImplementedException();
                }
                // namapování OutputVars
                tempVars.AddOrUpdateRange(aar.getOutputVariables(ar.outputData));
            }
        }
    }
}
