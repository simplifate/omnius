using FSS.Omnius.BussinesObjects.Common;

namespace FSS.Omnius.BussinesObjects.Service
{
    public interface IActionProvider
    {
        int ActionIdFrom { get;  }
        int ActionIdTo { get;  }
        IAction GetAction(int actionId);
    }
}