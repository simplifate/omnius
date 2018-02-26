using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IRecoveryService
    {
        void RecoverApplication(string jsonInput, bool force);
    }
}
