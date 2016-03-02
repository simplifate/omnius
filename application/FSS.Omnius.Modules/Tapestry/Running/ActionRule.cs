using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class ActionRule : ActionRuleBase
    {
        /// <summary>
        /// Check Conditions
        /// </summary>
        /// <param name="tempVars">temporary variables to check</param>
        /// <returns>are conditions ok?</returns>
        public bool CanRun(Dictionary<string, object> tempVars)
        {
            bool not = Condition[0] == '!';
            KeyValueString condition = new KeyValueString(Condition);
            bool result = condition.CompareResolved(tempVars);
            return not ? !result : result;
        }

        /// <summary>
        /// Run function to prepare conditions
        /// </summary>
        /// <param name="tempVars"></param>
        public void PreRun(ActionResultCollection results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order < PreFunctionCount).OrderBy(aar => aar.Order);

            InnerRun(results, aars);
        }

        public override void Run(ActionResultCollection results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order > PreFunctionCount);

            InnerRun(results, aars);
        }

        public void ReverseRun(ActionResultCollection results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order > PreFunctionCount);

            ReverseInnerRun(results, aars);
        }
    }
}
