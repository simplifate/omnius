using System;
using FSS.Omnius.Actions.ReservationSystem.Service;
using FSS.Omnius.BussinesObjects.Actions;
using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.Service;

namespace FSS.Omnius.Actions.ReservationSystem.Actions
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