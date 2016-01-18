using FSS.Omnius.Modules.Tapestry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class Block : ActionRuleBase
    {
        public override void Run(ActionResultCollection results)
        {
            InnerRun(results, PreBlockActions);
        }
    }
}
