using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBItem : IToJson
    {
        public DBItem(DBConnection db)
        {
            _db = db;
        }
        public DBItem(DBConnection db, Dictionary<string, object> dict)
        {
            _db = db;
            _properties = dict;
        }

        public Tabloid tabloid { get; set; }

        private DBConnection _db;
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private Dictionary<string, object> _foreignKeys = new Dictionary<string, object>();
        
        public object this[string propertyName]
        {
            get
            {
                // property with table
                if (propertyName.Contains('.'))
                {
                    if (_properties.ContainsKey(propertyName))
                        return _properties[propertyName];
                }
                // only property
                else
                {
                    string realPropertyName = $"{tabloid.Name}.{propertyName}";
                    if (_properties.ContainsKey(realPropertyName))
                        return _properties[realPropertyName];
                }
                
                // nothing...
                return null;
            }
            set
            {
                if (propertyName.Contains('.'))
                    _properties[propertyName] = value;
                else
                    _properties[$"{tabloid?.Name}.{propertyName}"] = value;
            }
        }

        public bool HasProperty(string propertyName)
        {
            if (propertyName.Contains('.'))
            {
                return _properties.ContainsKey(propertyName);
            }

            return _properties.ContainsKey($"{tabloid.Name}.{propertyName}");
        }

        public IEnumerable<string> getFullColumnNames()
        {
            return _properties.Keys;
        }
        public List<string> getColumnNames()
        {
            List<string> result = new List<string>();
            foreach(string column in _properties.Keys)
            {
                string[]pair = column.Split('.');
                if (tabloid == null && pair.Length < 2)
                {
                    result.Add(column);
                }
                else if (pair.FirstOrDefault() == tabloid?.Name)
                {
                    result.Add(pair[1]);
                }
            }

            return result;
        }

        public IEnumerable<string> getColumnDisplayNames()
        {
            DBEntities e = DBEntities.instance;
            DbTable table = e.Applications.Find(_db.Application.Id).DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault()?.Tables.FirstOrDefault(t => t.Name == tabloid.Name);
            if (table != null)
                return table.Columns.Select(c => c.DisplayName ?? c.Name);

            return getColumnNames();
        }

        public List<object> getAllProperties()
        {
            return _properties.Values.ToList();
        }

        public JToken ToJson()
        {
            JObject result = new JObject();

            foreach(var pair in _properties)
            {
                result.Add(pair.Key, new JValue(pair.Value));
            }

            return result;
        }

        public override string ToString() => $"DBItem [{tabloid.Name}][{_db.Application.Name}]: {string.Join(",", _properties.Select(p => $"{p.Key} => {p.Value.ToString()}"))}";
    }
}
