﻿using FSS.FSPOC.Actions.ReservationSystem.Actions;
using FSS.FSPOC.BussinesObjects.Actions;
using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.Actions.ReservationSystem.Service
{
    public class ReservationSystemActionProvider : IReservationSystemActionProvider
    {
        public int ActionIdFrom => 1;
        public int ActionIdTo => 10;

        public IAction GetAction(int actionId)
        {
            return new CheckConfigurationSettings();
        }
    }
}