using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron
{
    public class DBItem
    {
        public DBTable table { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private Dictionary<string, object> _foreignKeys = new Dictionary<string, object>();
        public object this[string propertyName]
        {
            get
            {
                // property
                if (_properties.ContainsKey(propertyName))
                    return _properties[propertyName];

                // foreign keys
                DBForeignKey fk = null;
                if (_foreignKeys.ContainsKey(propertyName) || (fk = table.foreignKeys.FirstOrDefault(fkey => fkey.name == propertyName)) != null)
                {
                    if (!_foreignKeys.ContainsKey(propertyName))
                    {
                        // read data
                        DBTable targetTable = DBTable.GetTable(fk.targetTable);
                        _foreignKeys[propertyName] = targetTable.Select().where(c => c.column(fk.targetColumn).Equal(this[fk.sourceColumn])).ToList();
                    }

                    return _foreignKeys[propertyName];
                }

                // nothing...
                return null;
            }
            set
            {
                _properties[propertyName] = value;
            }
        }

        public List<string> getColumnNames()
        {
            return _properties.Keys.ToList();
        }
    }
}
