using Newtonsoft.Json;
using System.IO;
using System.Linq;
using FSS.FSPOC.BussinesObjects.DAL;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public class BackupGeneratorService : IBackupGeneratorService
    {
        public void ExportAllDatabaseDesignerData(string filename)
        {
            using (var context = new OmniusDbContext())
            {
                var commits = from c in context.DbSchemeCommits orderby c.Timestamp descending select c;
                string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                File.WriteAllText(filename, jsonOutput);
            }
        }
    }
}
