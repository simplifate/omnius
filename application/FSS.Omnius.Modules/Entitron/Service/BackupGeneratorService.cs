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
            var result = base.CreateProperties(type, memberSerialization);
            result = result.Where(p => !p.AttributeProvider.GetAttributes(true).Any(a =>
                        a.GetType() == typeof(ImportExportIgnoreAttribute)
                        && !(a as ImportExportIgnoreAttribute).IsKey
                        && !(a as ImportExportIgnoreAttribute).IsLinkKey
                    )).ToList();

            return result;
        }
    }
}
