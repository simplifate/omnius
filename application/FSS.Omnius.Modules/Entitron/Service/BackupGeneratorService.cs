using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class BackupGeneratorService : IBackupGeneratorService
    {
        public BackupGeneratorService(DBEntities appContext)
        {
            _context = appContext;
        }
        
        private DBEntities _context;
        private DBCommandSet _db;

        public void ExportAllDatabaseDesignerData(string filename)
        {
            var commits = from c in _context.DBSchemeCommits orderby c.Timestamp descending select c;
            string jsonOutput = JsonConvert.SerializeObject(commits.ToList(), Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            File.WriteAllText(filename, jsonOutput);
        }

        public string ExportApplication(int id)
        {
            return ExportApplication(id, new NameValueCollection());
        }

        public string ExportApplication(int id, NameValueCollection form) 
        {
            string[] toExport = form.AllKeys;

            _db = DBCommandSet.GetDBCommandSet(_context.Applications.Find(id).DB_Type);
            JObject result = new JObject();
            Queue<Type> queue = new Queue<Type>(RecoveryService.Roots);
            Dictionary<Type, HashSet<int>> ids = new Dictionary<Type, HashSet<int>>();

            foreach(Type root in RecoveryService.Roots)
            {
                // app -> by id
                if (root == typeof(Application))
                    ids.Add(root, new HashSet<int> { id });
                // others -> all
                else
                {
                    ids.Add(root, new HashSet<int>());
                    foreach(IEntity ent in _context.Set(root))
                    {
                        ids[root].Add(ent.GetId());
                    }
                }
            }

            Type currentType = queue.Dequeue();
            while (currentType != null)
            {
                string name = currentType.Name;
                /// parent
                string parentPropKey;
                ImportExportAttribute parentAttribute;
                if (RecoveryService.Roots.Contains(currentType))
                {
                    parentPropKey = RecoveryService.PrimaryKey;
                    parentAttribute = new ImportExportAttribute(ELinkType.Parent, currentType);
                }
                else
                {
                    try
                    {
                        PropertyInfo parentProp = currentType.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<ImportExportAttribute>()?.Type == ELinkType.Parent && p.GetCustomAttribute<ImportExportAttribute>().KeyFor.Any());
                        parentPropKey = parentProp.Name;
                        parentAttribute = parentProp.GetCustomAttribute<ImportExportAttribute>();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Could not find parent of type [{currentType.Name}]", ex);
                    }
                }
                Type parentType = parentAttribute.KeyFor.First();
                // we don't have required types
                if (!ids.ContainsKey(parentType))
                    throw new Exception($"We don't have parent type[{parentType.Name}]!");

                /// get items
                // skip if there are no parent (skip also children)
                if (ids[parentType].Any())
                {
                    string tableName = currentType.GetCustomAttribute<TableAttribute>().Name;
                    string sqlQuery = parentAttribute.exportCount != 0
                        ? $"SELECT * " +
                            $"FROM {_db.AddQuote(tableName)} {_db.AddQuote("table1")} " +
                            $"WHERE {_db.AddQuote("table1")}.{_db.AddQuote(parentPropKey)} IN ({string.Join(",", ids[parentType])}) AND {_db.AddQuote("table1")}.{_db.AddQuote(RecoveryService.PrimaryKey)} IN " +
                            $"(SELECT TOP({parentAttribute.exportCount}) {_db.AddQuote(RecoveryService.PrimaryKey)} FROM {_db.AddQuote(tableName)} {_db.AddQuote("table2")} " +
                            $" WHERE {_db.AddQuote("table2")}.{_db.AddQuote(parentPropKey)} = {_db.AddQuote("table1")}.{_db.AddQuote(parentPropKey)} " +
                            $" ORDER BY {_db.AddQuote("table2")}.{_db.AddQuote(parentAttribute.exportOrderColumn)} {(parentAttribute.exportOrderDesc ? "DESC" : "ASC")})"
                        : $"SELECT * " +
                            $"FROM {_db.AddQuote(tableName)} " +
                            $"WHERE {_db.AddQuote(parentPropKey)} IN ({string.Join(",", ids[parentType])})";
                    try
                    {
                        var query = _context.Database.SqlQuery(currentType, sqlQuery);
                        ids[currentType] = new HashSet<int>();
                        JArray items = new JArray();
                        foreach (IEntity row in query)
                        {
                            ids[currentType].Add(row.GetId());

                            items.Add(row.ToJson());
                        }
                        result.Add(currentType.Name, items);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Sql exception - Type[{currentType}]; query [{sqlQuery}]", ex);
                    }

                    IEnumerable<PropertyInfo> childProperties = currentType.GetProperties().Where(p => { var attr = p.GetCustomAttribute<ImportExportAttribute>(); return attr != null && attr.Type == ELinkType.Child && (attr.Branch == null || toExport.Contains(attr.Branch)); });
                    foreach (PropertyInfo prop in childProperties)
                    {
                        if (prop.PropertyType.GetGenericArguments().Count() > 0)
                            queue.Enqueue(prop.PropertyType.GetGenericArguments()[0]);
                        else
                            queue.Enqueue(prop.PropertyType);
                    }
                }
                
                /// next item
                try
                {
                    currentType = queue.Dequeue();
                }
                catch(InvalidOperationException)
                {
                    currentType = null;
                }
            }

            return JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
    }
}
