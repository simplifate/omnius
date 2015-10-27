using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public interface IDatabaseGenerateService
    {
        void GenerateDatabase(DbSchemeCommit dbSchemeCommit);
    }
}
