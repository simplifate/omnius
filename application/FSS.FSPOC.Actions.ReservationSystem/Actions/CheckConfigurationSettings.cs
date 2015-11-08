using System;
using FSS.FSPOC.Actions.ReservationSystem.Service;
using FSS.FSPOC.BussinesObjects.Actions;
using FSS.FSPOC.BussinesObjects.Common;
using FSS.FSPOC.BussinesObjects.Service;

namespace FSS.FSPOC.Actions.ReservationSystem.Actions
{
    public class CheckConfigurationSettings : IAction
    {
        public CheckConfigurationSettings(IReservationSystemService reservationSystemService,
            IActionService actionService)
        {
            if (reservationSystemService == null) throw new ArgumentNullException(nameof(reservationSystemService));
            ReservationSystemService = reservationSystemService;
            ActionService            = actionService;
        }

        private IReservationSystemService ReservationSystemService { get; }
        private IActionService ActionService { get; set; }

        public ResultAction Run(object paramActin = null)
        {
            var commonParamApplication = ActionService.GetParam<CommonParamApplication>();
            if (commonParamApplication != null)
            {
                var userId = commonParamApplication.UserId;
            }

            var configSettings = ReservationSystemService.ConfigSettings;
            if (configSettings == null)
            {
                ReservationSystemService.FetchConfigSettings();
                configSettings = ReservationSystemService.ConfigSettings;
            }
            //TODO zde bude provedeni overeni vuci konfiguracnimu nastaveni
            return new ResultAction();
        }
    }
}