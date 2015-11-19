using System;
using FSS.Omnius.Actions.ReservationSystem.Actions;
using FSS.Omnius.BussinesObjects.Actions;
using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.Service;

namespace FSS.Omnius.Actions.ReservationSystem.Service
{
    public class ReservationSystemActionProvider : IReservationSystemActionProvider
    {
        public ReservationSystemActionProvider(IReservationSystemService reservationSystemService,
            IActionService actionService)
        {
            if (reservationSystemService == null) throw new ArgumentNullException(nameof(reservationSystemService));
            if (actionService == null) throw new ArgumentNullException(nameof(actionService));

            ReservationSystemService = reservationSystemService;
            ActionService            = actionService;
        }

        private IReservationSystemService ReservationSystemService { get; set; }
        private IActionService ActionService { get; set; }
        public int ActionIdFrom => 1;
        public int ActionIdTo => 10;

        public IAction GetAction(int actionId)
        {
            return new CheckConfigurationSettings(ReservationSystemService,ActionService);
        }
    }
}