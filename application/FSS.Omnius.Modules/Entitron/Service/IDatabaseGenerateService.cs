using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public interface IDatabaseGenerateService
    {
        void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core);
    }
}
