using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.BussinesObjects.Actions
{
    public class CommonActionsProvider : ICommonActionsProvider
    {
        public int ActionIdFrom  => 100;
        public int ActionIdTo => 200;
        public IAction GetAction(int actionId)
        {
            return new SendNotification();
        }
    }
}