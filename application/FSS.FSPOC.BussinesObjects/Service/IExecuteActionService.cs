using System.Collections.Generic;
using FSS.Omnius.BussinesObjects.Common;

namespace FSS.Omnius.BussinesObjects.Service
{
    public interface IExecuteActionService
    {
        IEnumerable<ResultAction> RunAction(int actionRuleId,object sourceAction);
    }
}