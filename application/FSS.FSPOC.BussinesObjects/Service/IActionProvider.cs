using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IActionProvider
    {
        int ActionIdFrom { get;  }
        int ActionIdTo { get;  }
        IAction GetAction(int actionId);
    }
}