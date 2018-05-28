using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class RecoveryService : IRecoveryService
    {
        public const string PrimaryKey = "Id";
        public static Type[] Roots => new Type[] { typeof(Application), typeof(Entity.Athena.Graph) };

        private DBEntities _context;
        private DBCommandSet _db;
        private string _connectionString;
        private Queue<Type> _queue;
        private Dictionary<Type, Dictionary<int, int>> _ids;
        private Dictionary<Type, Dictionary<int, Dictionary<string, object>>> _optionalValues;

        public RecoveryService()
        {
            _db = DBCommandSet.GetDBCommandSet(Entitron.DefaultDBType);
            _connectionString = Entitron.EntityConnectionString;
            _queue = new Queue<Type>(Roots);
            _ids = new Dictionary<Type, Dictionary<int, int>>();
            _optionalValues = new Dictionary<Type, Dictionary<int, Dictionary<string, object>>>();
        }

        //This method will take a json String and return Application object
        public void RecoverApplication(string jsonInput, bool force)
        {
            JToken inputJson = JToken.Parse(jsonInput);

            /// change app name
            JToken application = inputJson["Application"].First;
            string applicationName = (string)(application["Name"] as JValue).Value;
            string applicationDisplayName = (string)(application["DisplayName"] as JValue).Value;
            string tempAppName = $"{applicationName}_importing";

            (application["Name"] as JValue).Value = tempAppName;
            (application["DisplayName"] as JValue).Value = $"{applicationDisplayName} - importing";

            /// get context
            _context = DBEntities.appInstance(DBEntities.instance.Applications.SingleOrDefault(a => a.Name == applicationName));

            /// if temp app exists
            Application tempApp = _context.Applications.SingleOrDefault(a => a.Name == tempAppName);
            if (tempApp != null)
            {
                if (force)
                {
                    _context.Applications.Remove(tempApp);
                    _context.SaveChanges();
                }
                else
                    throw new Exception("Temporary application already exists!");
            }

            /// all types
            Type currentType = _queue.Dequeue();
            Type cycleDetector = null;
            while (currentType != null)
            {
                try
                {
                    // no data to import
                    if (inputJson[currentType.Name] == null)
                        throw new NextType();

                    /// dependency & cycle
                    if (!getRequiredProperties(currentType).All(t => t.GetCustomAttribute<ImportExportAttribute>().KeyFor.All(tt => _ids.ContainsKey(tt)) || t.PropertyType == currentType))
                    {
                        if (cycleDetector == currentType)
                            throw new Exception($"Cycle detected [{currentType}, {string.Join(", ", _queue.Select(t => t.Name))}]");
                        else if (cycleDetector == null)
                            cycleDetector = currentType;

                        // next item
                        _queue.Enqueue(currentType);
                        throw new NextType();
                    }
                    else
                        cycleDetector = null;

                    /// Foreach entity: Get object & Change required Ids
                    _ids[currentType] = new Dictionary<int, int>();
                    // normal
                    createEntity(inputJson[currentType.Name], currentType);

                    /// Children properties
                    IEnumerable<PropertyInfo> childProperties = getChildProperties(currentType);
                    foreach (PropertyInfo prop in childProperties)
                    {
                        Type propType = prop.PropertyType.GetGenericArguments().Count() > 0
                            ? prop.PropertyType.GetGenericArguments()[0]
                            : prop.PropertyType;

                        _queue.Enqueue(propType);
                    }

                    /// next item
                    throw new NextType();
                }
                catch (NextType)
                {
                    try
                    {
                        currentType = _queue.Dequeue();
                    }
                    catch (InvalidOperationException)
                    {
                        currentType = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in type [{currentType.Name}]", ex);
                }
            }

            /// Optional links
            foreach (var typePair in _optionalValues)
            {
                // optional columns
                string sql = $"UPDATE {_db.AddQuote(typePair.Key.GetCustomAttribute<TableAttribute>().Name)} SET {string.Join(",", getOptionalProperties(typePair.Key).Select(p => $"{_db.AddQuote(p.Name)} = @{p.Name}"))} WHERE {_db.AddQuote(PrimaryKey)} = @{PrimaryKey}";

                // data
                foreach (var oldIdPair in typePair.Value)
                {
                    IDbCommand command = _db.Command;
                    command.CommandText = sql;
                    command.AddParam(PrimaryKey, _ids[typePair.Key][oldIdPair.Key]);

                    foreach (PropertyInfo prop in getOptionalProperties(typePair.Key))
                    {
                        ImportExportAttribute attr = prop.GetCustomAttribute<ImportExportAttribute>();
                        try
                        {
                            object originValue = oldIdPair.Value[prop.Name];
                            //if (originValue == null)
                            //    continue;

                            // single key for multiple property
                            if (attr.KeyForMultiple_property != null)
                            {
#warning TODO: KeyForMultiple_property
                                //int separator = (int)type.GetProperties().SingleOrDefault(p => p.Name == attr.KeyForMultiple_property).GetValue(pair.Key);

                                //prop.SetValue(pair.Key, _ids[attr.KeyFor[separator]][(originValue as int?).Value]);
                            }
                            else
                            {
                                Type targetType = attr.KeyFor.Single();
                                // multiple ids separated by comma
                                if (attr.MultipleIdInString)
                                {
                                    string ids = (string)originValue;
                                    if (!string.IsNullOrWhiteSpace(ids))
                                    {
                                        IEnumerable<int> idsInt = ids.Split(',').Select(id => Convert.ToInt32(id));
                                        IEnumerable<int> newIds = idsInt.Select(i => _ids[targetType][i]);

                                        command.AddParam(prop.Name, string.Join(",", newIds));
                                    }
                                    else
                                    {
                                        command.AddParam(prop.Name, DBNull.Value);
                                    }
                                }
                                // typical id
                                else
                                {
                                    if (originValue != null)
                                        command.AddParam(prop.Name, _ids[targetType][Convert.ToInt32(originValue)]);
                                    else
                                        command.AddParam(prop.Name, DBNull.Value);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Exception in Type[{typePair.Key.FullName}], optional param[{prop.Name}], {(oldIdPair.Value.ContainsKey(prop.Name) ? $"value[{oldIdPair.Value[prop.Name]}]" : "no value")}", ex);
                        }
                    }

                    using (IDbConnection connection = _db.Connection)
                    {
                        connection.ConnectionString = _connectionString;
                        connection.Open();

                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                }
            }
            _context.SaveChanges();

            /// everything ok -> merge Application
            Application newApp = _context.Applications.SingleOrDefault(a => a.Name == tempAppName);
            Application oldApp = _context.Applications.SingleOrDefault(a => a.Name == applicationName);
            // app exists -> merge
            if (oldApp != null)
            {
                IEnumerable<PropertyInfo> appChildProperties = getChildProperties(typeof(Application));
                foreach (PropertyInfo prop in appChildProperties)
                {
                    try
                    {
                        Type propType = prop.PropertyType.GetGenericArguments().Count() > 0
                            ? prop.PropertyType.GetGenericArguments()[0]
                            : prop.PropertyType;

                        if (inputJson[propType.Name] != null)
                        {
                            // remove old
                            IEnumerable<dynamic> items = (IEnumerable<dynamic>)prop.GetValue(oldApp);
                            if (items != null)
                            {
                                _context.Set(propType).RemoveRange(items);
                                _context.SaveChanges();
                            }
                            // move new
                            PropertyInfo parentProperty = propType.GetProperties().Single(p => { ImportExportAttribute attr = p.GetCustomAttribute<ImportExportAttribute>(); return attr != null && attr.Type == ELinkType.Parent && attr.KeyFor.FirstOrDefault() == typeof(Application); });
                            foreach (var idPair in _ids[propType])
                            {
                                using (IDbConnection connection = _db.Connection)
                                {
                                    connection.ConnectionString = _connectionString;
                                    connection.Open();

                                    IDbCommand command = _db.Command;
                                    command.CommandText = $"UPDATE {_db.AddQuote(propType.GetCustomAttribute<TableAttribute>().Name)} SET {_db.AddQuote(parentProperty.Name)} = @{prop.Name} WHERE {_db.AddQuote(PrimaryKey)} = @{PrimaryKey}";
                                    command.Connection = connection;
                                    command.AddParam(prop.Name, oldApp.Id);
                                    command.AddParam(PrimaryKey, idPair.Value);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error merging applications: property[{prop.Name}]. See inner exception.", ex);
                    }
                }
                _context.Applications.Remove(newApp);
            }
            // app doesn't exists -> rename
            else
            {
                newApp.Name = applicationName;
                newApp.DisplayName = applicationDisplayName;
            }
            _context.SaveChanges();
        }

        private void createEntity(IEnumerable<JToken> jsonEntities, Type currentType)
        {
            /// init
            IEnumerable<PropertyInfo> currentRequiredProperties = getRequiredProperties(currentType);
            IEnumerable<PropertyInfo> optionalProperties = getOptionalProperties(currentType);
            IEnumerable<PropertyInfo> nonOptionalProperties = getNonOptionalProperties(currentType);

            List<object> rootKeyPropertyValues = null;
            PropertyInfo keyProperty = currentType.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<ImportExportPropertyAttribute>()?.IsKey == true);

            /// generate sql
            string tableName = _db.AddQuote(currentType.GetCustomAttribute<TableAttribute>().Name);
            string sql = _db.Type == ESqlType.MSSQL
                ? $"INSERT INTO {tableName}({string.Join(",", nonOptionalProperties.Select(p => _db.AddQuote(p.Name)))}) OUTPUT inserted.{_db.AddQuote(PrimaryKey)} VALUES({string.Join(",", nonOptionalProperties.Select(c => $"@{c.Name}"))});"
                : $"INSERT INTO {tableName}({string.Join(",", nonOptionalProperties.Select(p => _db.AddQuote(p.Name)))}) VALUES({string.Join(",", nonOptionalProperties.Select(c => $"@{c.Name}"))}); SELECT LAST_INSERT_ID() {PrimaryKey}";
            string updateSql = keyProperty == null
                ? ""
                : _db.Type == ESqlType.MSSQL
                    ? $"UPDATE {tableName} SET {string.Join(",", nonOptionalProperties.Select(p => $"{_db.AddQuote(p.Name)} = @{p.Name}"))} OUTPUT inserted.{_db.AddQuote(PrimaryKey)} WHERE {_db.AddQuote(keyProperty.Name)} = @{keyProperty.Name}"
                    : $"UPDATE {tableName} SET {string.Join(",", nonOptionalProperties.Select(p => $"{_db.AddQuote(p.Name)} = @{p.Name}"))} WHERE {_db.AddQuote(keyProperty.Name)} = @{keyProperty.Name}; SELECT {PrimaryKey} FROM {tableName} WHERE {_db.AddQuote(keyProperty.Name)} = @{keyProperty.Name}";

            /// updating Root entity
            if (Roots.Contains(currentType) && keyProperty != null)
            {
                // read from json
                var jsonValues = jsonEntities.Select(e => e[keyProperty.Name].ToObject(keyProperty.PropertyType));

                // read from db
                using (IDbConnection connection = _db.Connection)
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    IDbCommand command = _db.Command;

                    List<string> paramNames = new List<string>();
                    int i = 0;
                    foreach (var jsonValue in jsonValues)
                    {
                        string param = $"param{i}";
                        command.AddParam(param, jsonValue);
                        paramNames.Add(param);
                        i++;
                    }

                    command.CommandText = $"SELECT {_db.AddQuote(keyProperty.Name)} FROM {_db.AddQuote(currentType.GetCustomAttribute<TableAttribute>().Name)} WHERE {_db.AddQuote(keyProperty.Name)} IN ({string.Join(",", paramNames.Select(p => $"@{p}"))})";
                    command.Connection = connection;
                    using (var reader = command.ExecuteReader())
                    {
                        rootKeyPropertyValues = new List<object>();
                        while (reader.Read())
                        {
                            rootKeyPropertyValues.Add(reader[keyProperty.Name]);
                        }
                    }
                }
            }

            /// create entity
            using (IDbConnection connection = _db.Connection)
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                foreach (JToken jsonEntity in jsonEntities)
                {
                    try
                    {
                        int oldId = jsonEntity[PrimaryKey].ToObject<int>();

                        /// change required ids
                        HashSet<string> propertiesWithCorrectValues = new HashSet<string>();
                        foreach (PropertyInfo prop in currentRequiredProperties)
                        {
                            try
                            {
                                ImportExportAttribute attr = prop.GetCustomAttribute<ImportExportAttribute>();

                                int? originId = jsonEntity[prop.Name].ToObject<int?>();
                                // skip items without required items
                                // this item has null or invalid FK
                                if (attr.skipItem && (originId == null || !_ids[attr.KeyFor.Single()].ContainsKey(originId.Value)))
                                {
                                    bool otherPropHasValue = false;
                                    foreach (string pairPropName in attr.skipPair)
                                    {
                                        PropertyInfo pairProp = currentType.GetProperty(pairPropName);
                                        int? otherPropValue = jsonEntity[pairPropName].ToObject<int?>();
                                        if (propertiesWithCorrectValues.Contains(pairPropName) || (otherPropValue != null && _ids[pairProp.GetCustomAttribute<ImportExportAttribute>().KeyFor.Single()].ContainsKey(otherPropValue.Value)))
                                        {
                                            otherPropHasValue = true;
                                            break;
                                        }
                                    }

                                    // has pair property correct value?
                                    if (otherPropHasValue)
                                        jsonEntity[prop.Name] = null;
                                    // any pair property hasn't correct value -> skip
                                    else
                                        throw new NextEntity();
                                }
                                // property has correct value || you shouldn't skip it
                                else
                                {
                                    jsonEntity[prop.Name] = _ids[attr.KeyFor.Single()][originId.Value];
                                    propertiesWithCorrectValues.Add(prop.Name);
                                }
                            }
                            catch (NextEntity)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Exception in Type[{currentType.FullName}], optional param[{prop.Name}], entity", ex);
                            }
                        }

                        /// save optional property ids
                        foreach (PropertyInfo prop in optionalProperties)
                        {
                            object originValue = jsonEntity[prop.Name].ToObject<object>();
                            //if (originValue != null)
                            //{
                            if (!_optionalValues.ContainsKey(currentType))
                                _optionalValues[currentType] = new Dictionary<int, Dictionary<string, object>>();
                            if (!_optionalValues[currentType].ContainsKey(oldId))
                                _optionalValues[currentType][oldId] = new Dictionary<string, object>();

                            _optionalValues[currentType][oldId].Add(prop.Name, originValue);
                            //}
                        }

                        
                        /// insert
                        IDbCommand command = _db.Command;
                        command.Connection = connection;
                        command.CommandText = (rootKeyPropertyValues != null && rootKeyPropertyValues.Contains(jsonEntity[keyProperty.Name].ToObject(keyProperty.PropertyType)))
                            ? updateSql // update root
                            : sql; // insert
                        foreach (PropertyInfo prop in nonOptionalProperties)
                        {
                            command.AddParam(prop.Name, jsonEntity[prop.Name].ToObject<object>() ?? DBNull.Value);
                        }

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            /// get Id
                            reader.Read();
                            _ids[currentType].Add(jsonEntity[PrimaryKey].ToObject<int>(), Convert.ToInt32(reader[PrimaryKey]));
                        }
                    }
                    catch (NextEntity)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// get child-link properties
        /// </summary>
        private IEnumerable<PropertyInfo> getChildProperties(Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<ImportExportAttribute>()?.Type == ELinkType.Child);
        }
        /// <summary>
        /// get parent & required-link properties
        /// </summary>
        private IEnumerable<PropertyInfo> getRequiredProperties(Type type)
        {
            // ignores object link properties
            return type.GetProperties().Where(p =>
            {
                ImportExportAttribute attr = p.GetCustomAttribute<ImportExportAttribute>();
                return
                    attr != null
                    && attr.KeyFor.Any()
                    && (attr.Type == ELinkType.Parent
                        || attr.Type == ELinkType.LinkRequired);
            });
        }
        /// <summary>
        /// get optional-link properties
        /// </summary>
        private IEnumerable<PropertyInfo> getOptionalProperties(Type type)
        {
            return type.GetProperties().Where(p =>
            {
                ImportExportAttribute attr = p.GetCustomAttribute<ImportExportAttribute>();
                return
                    attr != null
                    && attr.KeyFor.Any()
                    && attr.Type == ELinkType.LinkOptional;
            });
        }
        /// <summary>
        /// get writable non-link properties (without primary key), required-link and parent-link properties
        /// </summary>
        private IEnumerable<PropertyInfo> getNonOptionalProperties(Type type)
        {
            return type.GetProperties().Where(p =>
            {
                ImportExportAttribute attr = p.GetCustomAttribute<ImportExportAttribute>();
                return
                    p.Name != PrimaryKey
                    && (DataType.BaseTypes.Contains(p.PropertyType)
                        || p.PropertyType.IsEnum)
                    && p.CanWrite
                    && (attr == null
                        || (attr.KeyFor.Any()
                            && attr.Type != ELinkType.LinkOptional));
            });
        }

        private class NextType : Exception
        {
        }
        private class NextEntity : Exception
        {
        }
    }
}
