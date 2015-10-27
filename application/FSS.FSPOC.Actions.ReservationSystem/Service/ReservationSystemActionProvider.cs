using FSS.FSPOC.BussinesObjects.Common;
using FSS.FSPOC.BussinesObjects.Service;

namespace FSS.FSPOC.Actions.ReservationSystem.Service
{
    public class ReservationSystemActionProvider :IActionProvider
    {
        public int ActionIdFrom => 1;

        public int ActionIdTo => 10;

        public IAction GetAction(int actionId)
        {
            throw new System.NotImplementedException();
        }
    }
}