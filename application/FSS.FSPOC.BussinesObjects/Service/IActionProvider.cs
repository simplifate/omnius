using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IActionProvider
    {
        int ActionIdFrom { get; set; }
        int ActionIdTo { get; set; }
        IAction GetAction(int actionId);
    }
}