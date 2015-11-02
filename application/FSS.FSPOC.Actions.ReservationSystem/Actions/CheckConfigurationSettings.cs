using System;
using FSS.FSPOC.Actions.ReservationSystem.Service;
using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.Actions.ReservationSystem.Actions
{
    public class CheckConfigurationSettings : IAction
    {
        public CheckConfigurationSettings(IReservationSystemService reservationSystemService)
        {
            if (reservationSystemService == null) throw new ArgumentNullException(nameof(reservationSystemService));
            ReservationSystemService = reservationSystemService;
        }

        private IReservationSystemService ReservationSystemService { get; }

        public ResultAction Run(object paramActin = null)
        {
            var configSettings = ReservationSystemService.ConfigSettings;
            if (configSettings==null)
            {
                ReservationSystemService.FetchConfigSettings();
                configSettings = ReservationSystemService.ConfigSettings;

            }
            //TODO zde bude provedeni overeni vuci konfiguracnimu nastaveni
            return new ResultAction();
        }
    }
}