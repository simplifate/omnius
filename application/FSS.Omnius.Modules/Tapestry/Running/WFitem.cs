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

    public partial class TapestryDesignerConnection
    {
        public WFitem GetTarget(TapestryDesignerWorkflowRule workflowRule, DBEntities context)
        {
            return
                (TargetType == 0)
                    ? (WFitem)context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == Target)
                    : context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == Target);
        }
        public WFitem GetSource(TapestryDesignerWorkflowRule workflowRule, DBEntities context)
        {
            return
                (SourceType == 0)
                    ? (WFitem)context.TapestryDesignerWorkflowItems.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == Source)
                    : context.TapestryDesignerWorkflowSymbols.SingleOrDefault(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && i.Id == Source);
        }
    }
}
