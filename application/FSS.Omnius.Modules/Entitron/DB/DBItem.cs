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
        public DBItem(DBConnection db, Tabloid tabloid)
        {
            _db = db;
            Tabloid = tabloid;
            _properties = new Dictionary<string, object>();
            _foreignKeys = new Dictionary<string, object>();
        }
        public DBItem(DBConnection db, Tabloid tabloid, Dictionary<string, object> dict)
        {
            _db = db;
            Tabloid = tabloid;
            _properties = new Dictionary<string, object>();
            _foreignKeys = new Dictionary<string, object>();

            foreach (var pair in dict)
            {
                string key = pair.Key;
                if (!key.Contains("."))
                    key = $"{tabloid.Name}.{key}";
                _properties.Add(key, pair.Value);
            }
        }

        public Tabloid Tabloid { get; set; }

        private DBConnection _db;
        private Dictionary<string, object> _properties;
        private Dictionary<string, object> _foreignKeys;
        
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
                    string realPropertyName = $"{Tabloid?.Name}.{propertyName}";
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
                    _properties[$"{Tabloid?.Name}.{propertyName}"] = value;
            }
        }

        public bool HasProperty(string propertyName)
        {
            if (propertyName.Contains('.'))
            {
                return _properties.ContainsKey(propertyName);
            }

            return _properties.ContainsKey($"{Tabloid?.Name}.{propertyName}");
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
                if (Tabloid == null && pair.Length < 2)
                {
                    result.Add(column);
                }
                else if (pair.FirstOrDefault() == (Tabloid?.Name ?? ""))
                {
                    result.Add(pair[1]);
                }
            }

            return result;
        }

        public IEnumerable<string> getColumnDisplayNames()
        {
            DBEntities e = DBEntities.instance;
            DbTable table = e.Applications.Find(_db.Application.Id).DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault()?.Tables.FirstOrDefault(t => t.Name == Tabloid?.Name);
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

        public override string ToString() => $"DBItem [{Tabloid?.Name}][{_db.Application.Name}]: {string.Join(",", _properties.Select(p => $"{p.Key} => {p.Value.ToString()}"))}";
    }
}
