using FSS.Omnius.Actions.ReservationSystem.BussinesObjects;

namespace FSS.Omnius.Actions.ReservationSystem.Service
{
    public class ReservationSystemService :IReservationSystemService
    {
        private ConfigSettings _configSettings;

        public ConfigSettings ConfigSettings => _configSettings;
        public void FetchConfigSettings()
        {
            _configSettings = new ConfigSettings
            {
                Schvalovatel = "pouze pro  test!!!!"
            };
        }
    }
}