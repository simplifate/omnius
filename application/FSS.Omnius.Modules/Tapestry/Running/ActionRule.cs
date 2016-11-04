using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class ActionRule : IActionRule
    {
        /// <summary>
        /// Run function to prepare conditions
        /// </summary>
        /// <param name="tempVars"></param>
        public void PreRun(ActionResult results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order < PreFunctionCount).OrderBy(aar => aar.Order).ToList();

            this.InnerRun(results, aars);
        }

        public void Run(ActionResult results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order > PreFunctionCount);

            this.InnerRun(results, aars);
        }

        public void ReverseRun(ActionResult results)
        {
            var aars = ActionRule_Actions.Where(aar => aar.Order > PreFunctionCount);

            this.ReverseInnerRun(results, aars);
        }
    }
}
