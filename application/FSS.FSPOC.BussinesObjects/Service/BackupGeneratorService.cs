using Newtonsoft.Json;
using System.IO;
using System.Linq;
using FSS.Omnius.Entitron.Entity;

namespace FSS.Omnius.BussinesObjects.Service
{
    public class BackupGeneratorService : IBackupGeneratorService
    {
        public void ExportAllDatabaseDesignerData(string filename)
        {
            using (var context = new DBEntities())
            {
                var commits = from c in context.DBSchemeCommits orderby c.Timestamp descending select c;
                string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                File.WriteAllText(filename, jsonOutput);
            }
        }
    }
}
