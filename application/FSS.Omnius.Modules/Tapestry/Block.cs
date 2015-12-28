using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class Block
    {
        public void RunPreActionRule()
        {
            if (PreBlockActionRule == null)
                return;

            PreBlockActionRule.MainRun();
        }
    }
}
