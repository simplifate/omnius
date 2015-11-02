using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSS.FSPOC.BussinesObjects.Actions;
using FSS.FSPOC.BussinesObjects.Common;
using FSS.FSPOC.BussinesObjects.Service;

namespace FSPOC_WebProject.Service
{
    public class FactoryAction : IFactoryAction
    {
        public FactoryAction()
        {
            CreateProviders();
        }

        private List<IActionProvider> ActionProviders { get; } = new List<IActionProvider>();

        public IAction GetAction(int actionId)
        {
            var actionProvider = ActionProviders.First(p => p.ActionIdFrom <= actionId && p.ActionIdTo > actionId);
            return actionProvider.GetAction(actionId);
        }

        private void CreateProviders()
        {
            var commonActionsProvider           = DependencyResolver.Current.GetService<ICommonActionsProvider>();
            var reservationSystemActionProvider = DependencyResolver.Current.GetService<IReservationSystemActionProvider>();

            if (commonActionsProvider != null)
            {
                ActionProviders.Add(commonActionsProvider);
            }
            if (reservationSystemActionProvider != null)
            {
                ActionProviders.Add(reservationSystemActionProvider);
            }
        }
    }
}