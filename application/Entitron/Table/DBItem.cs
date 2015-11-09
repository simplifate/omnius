using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Entitron.Sql;

namespace Entitron
{
    public class DBItem
    {
        public DBTable table { get; set; }

        private Dictionary<string, int> _properties = new Dictionary<string, int>();
        private Dictionary<string, object> _foreignKeys = new Dictionary<string, object>();
        private Dictionary<int, object> _idProperties = new Dictionary<int, object>(); 
        
        public object this[string propertyName]
        {
            get
            {
                // property
                if (_properties.ContainsKey(propertyName))
                    return _idProperties[_properties[propertyName]];

                // foreign keys
                DBForeignKey fk = null;
                if (_foreignKeys.ContainsKey(propertyName) || (fk = table.foreignKeys.FirstOrDefault(fkey => fkey.name == propertyName)) != null)
                {
                    if (!_foreignKeys.ContainsKey(propertyName))
                    {
                        // read data
                        _foreignKeys[propertyName] = fk.targetTable.Select().where(c => c.column(fk.targetColumn).Equal(this[fk.sourceColumn])).ToList();
                    }

                    return _foreignKeys[propertyName];
                }

                // nothing...
                return null;
            }
            set
            {
                if (!_properties.ContainsKey(propertyName))
                    throw new KeyNotFoundException();

                int columnId = _properties[propertyName];
                _idProperties[columnId] = value;
            }
        }

        public void createProperty(int columnId, string name, object value)
        {
            _idProperties.Add(columnId,value);
            _properties.Add(name,columnId);
        }


        public List<string> getColumnNames()
        {
            return _properties.Keys.ToList();
        }

        public List<object> getAllProperties()
        {
            return _idProperties.Values.ToList();
        } 
    }
}
