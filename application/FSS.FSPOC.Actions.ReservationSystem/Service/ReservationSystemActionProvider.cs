using System;
using FSS.FSPOC.Actions.ReservationSystem.Actions;
using FSS.FSPOC.BussinesObjects.Actions;
using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.Actions.ReservationSystem.Service
{
    public class ReservationSystemActionProvider : IReservationSystemActionProvider
    {
        public ReservationSystemActionProvider(IReservationSystemService reservationSystemService)
        {
            if (reservationSystemService == null) throw new ArgumentNullException(nameof(reservationSystemService));
            ReservationSystemService = reservationSystemService;
        }

        private IReservationSystemService ReservationSystemService { get; set; }
        public int ActionIdFrom => 1;
        public int ActionIdTo => 10;

        public IAction GetAction(int actionId)
        {
            return new CheckConfigurationSettings(ReservationSystemService);
        }
    }
}