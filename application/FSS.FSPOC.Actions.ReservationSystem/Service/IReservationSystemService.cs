using FSS.FSPOC.Actions.ReservationSystem.BussinesObjects;

namespace FSS.FSPOC.Actions.ReservationSystem.Service
{
    public interface IReservationSystemService
    {
        ConfigSettings ConfigSettings { get; }
        void FetchConfigSettings();
    }
}