using FSS.FSPOC.BussinesObjects.Common;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IFactoryAction
    {
        IAction GetAction(int actionId);
    }
}