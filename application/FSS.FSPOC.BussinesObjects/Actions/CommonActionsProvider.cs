using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.Service;

namespace FSS.Omnius.BussinesObjects.Actions
{
    public class CommonActionsProvider : ICommonActionsProvider
    {
        private IActionService ActionService { get; set; }

        public CommonActionsProvider(IActionService actionService)
        {
            ActionService = actionService;
        }

        public int ActionIdFrom => 100;
        public int ActionIdTo => 200;
        public IAction GetAction(int actionId)
        {
            return new SendNotification(ActionService);
        }
    }
}