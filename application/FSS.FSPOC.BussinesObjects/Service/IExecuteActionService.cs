using System.Collections.Generic;
using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IExecuteActionService
    {
        IEnumerable<ResultAction> RunAction(int actionRuleId,object sourceAction);
    }
}