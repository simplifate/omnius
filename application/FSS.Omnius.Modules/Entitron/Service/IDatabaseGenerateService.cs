using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IDatabaseGenerateService
    {
        void GenerateDatabase(DbSchemeCommit dbSchemeCommit, COREobject core);
    }
}
