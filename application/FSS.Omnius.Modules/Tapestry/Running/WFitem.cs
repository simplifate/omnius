using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public abstract partial class WFitem
    {
        public abstract int GetTypeId();
    }

    public partial class TapestryDesignerWorkflowItem
    {
        public override int GetTypeId()
        {
            return 0;
        }
    }

    public partial class TapestryDesignerWorkflowSymbol
    {
        public override int GetTypeId()
        {
            return 1;
        }
    }
}
