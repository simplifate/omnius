using FSS.Omnius.Actions.ReservationSystem.BussinesObjects;

namespace FSS.Omnius.Actions.ReservationSystem.Service
{
    public interface IReservationSystemService
    {
        ConfigSettings ConfigSettings { get; }
        void FetchConfigSettings();
    }
}