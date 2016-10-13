using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Compass.Service
{
    public interface IElasticService
    {
        void Index(List<FileMetadata> files);
        ElasticServiceFoundDocument[] Search(string query, string appName = null);
    }
}
