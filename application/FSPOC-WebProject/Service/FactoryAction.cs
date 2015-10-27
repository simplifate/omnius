using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSPOC_WebProject.Actions;
using FSS.FSPOC.Actions.ReservationSystem.Service;
using FSS.FSPOC.BussinesObjects.Common;
using FSS.FSPOC.BussinesObjects.Service;

namespace FSPOC_WebProject.Service
{
    public class FactoryAction : IFactoryAction
    {
        public FactoryAction(ISaveEntity saveEntity)
        {
            SaveEntity = saveEntity;

            //Todo predelat!!!!!
            var reservationSystemActionProvider =
                DependencyResolver.Current.GetService<IReservationSystemActionProvider>();
            if (reservationSystemActionProvider != null)
            {
                ActionProviders.Add(reservationSystemActionProvider);
            }
        }

        private List<IActionProvider> ActionProviders { get; } = new List<IActionProvider>();
        private ISaveEntity SaveEntity { get; set; }

        public IAction GetAction(int actionId)
        {
            var actionProvider = ActionProviders.First(p => p.ActionIdFrom <= actionId && p.ActionIdTo > actionId);
            return actionProvider.GetAction(actionId);
        }
    }
}