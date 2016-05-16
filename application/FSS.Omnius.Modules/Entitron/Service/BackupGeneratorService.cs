using Newtonsoft.Json;
using System.IO;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Service
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

        public string ExportApplication(int id) 
        {
            using (var context = new DBEntities())
            {
                var application = context.Applications.SingleOrDefault(a => a.Id == id);
                if (application != null)
                {
                    string jsonOutput = JsonConvert.SerializeObject(application, Formatting.Indented,
                    new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new IgnoreAttributeResolver() });
                    return jsonOutput;
                }
                else {
                    return "";
                }
            }
            
        }
    }

    class IgnoreAttributeResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var a = base.CreateProperties(type, memberSerialization);
            List<JsonProperty> b = new List<JsonProperty>();
            foreach(var q in a)
            {
                var q2 = type.BaseType.GetProperties().Where(u => u.Name == q.PropertyName);
                if (!q2.Any())
                    q2 = type.GetProperties().Where(u => u.Name == q.PropertyName);

                bool export = true;
                foreach (var q3 in q2)
                {
                    if (q3 != null && q3.GetCustomAttributes(true).Any(at => at.GetType() == typeof(ImportExportIgnoreAttribute)))
                        export = false;
                }
                if (export)
                    b.Add(q);
            }
            //= a.Where(p => type.BaseType.GetProperty(p.PropertyName) == null 
            //                    || !type.BaseType.GetProperty(p.PropertyName).GetCustomAttributes(true).Any(at => at.GetType() == typeof(ImportExportIgnoreAttribute))).ToList();
            return b;
        }
    }
}
