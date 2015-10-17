using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace FSPOC
{
    public class BackupGenerator
    {
        public void ExportAllDatabaseDesignerData(string filename)
        {
            using (var context = new DAL.WorkflowDbContext())
            {
                var commits = from c in context.DbSchemeCommits orderby c.Timestamp descending select c;
                string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                File.WriteAllText(filename, jsonOutput);
            }
        }
        public void ExportAllTapestryData(string filename)
        {
            using (var context = new DAL.TapestryDbContext())
            {
                var commits = from c in context.Commits orderby c.Timestamp descending select c;
                string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                File.WriteAllText(filename, jsonOutput);
            }
        }
    }
}
