using FSS.Omnius.BussinesObjects.Common;

namespace FSS.Omnius.BussinesObjects.Service
{
    public interface IFactoryAction
    {
        IAction GetAction(int actionId);
    }
}