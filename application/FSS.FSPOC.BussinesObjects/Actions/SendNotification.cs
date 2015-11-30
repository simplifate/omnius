using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.Service;

namespace FSS.Omnius.BussinesObjects.Actions
{
    internal class SendNotification : IAction
    {
        private IActionService ActionService { get; set; }

        public SendNotification(IActionService actionService)
        {
            ActionService = actionService;
        }

        public ResultAction Run(object paramActin = null)
        {
            ActionService.AddParam(new CommonParamApplication {UserId = 1});
            return new ResultAction();
        }
    }
}