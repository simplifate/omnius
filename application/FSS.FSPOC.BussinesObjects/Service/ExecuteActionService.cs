using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.DAL;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.BussinesObjects.Service
{
    public class ExecuteActionService : IExecuteActionService
    {
        public ExecuteActionService(IRepository<ActionRule_Action> actionActionRuleRepository,
            IFactoryAction factoryAction)
        {
            if (actionActionRuleRepository == null) throw new ArgumentNullException(nameof(actionActionRuleRepository));
            if (factoryAction == null) throw new ArgumentNullException(nameof(factoryAction));

            ActionActionRuleRepository = actionActionRuleRepository;
            FactoryAction              = factoryAction;
        }

        private IRepository<ActionRule_Action> ActionActionRuleRepository { get; }
        private IFactoryAction FactoryAction { get; set; }

        public IEnumerable<ResultAction> RunAction(int actionRuleId,object sourceAction)
        {
            

            var listResult = new List<ResultAction>();

            //pouze pro ukazku SMAZAT po predevedeni!!!!!!
            //comon Action
            var commonAction = FactoryAction.GetAction(100);
            listResult.Add(commonAction.Run(sourceAction));
            //reservatin system action
            var testAction = FactoryAction.GetAction(1);
            listResult.Add(testAction.Run(sourceAction));
            return listResult;
            //konec testu
            try
            {
                var actionActionRules = ActionActionRuleRepository.Get(a => a.ActionRuleId == actionRuleId,
                    q => q.OrderBy(a => a.Order), "Action");

                //listResult.AddRange(actionActionRules
                //    .Select(actionActionRule => FactoryAction.GetAction(actionActionRule.Action.IdentifierAction))
                //    .Select(action => action.Run(sourceAction)).Where(resultAction => resultAction != null));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return listResult;
        }
    }
}