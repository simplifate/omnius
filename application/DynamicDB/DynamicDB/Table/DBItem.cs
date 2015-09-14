using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB
{
    public class DBItem
    {
        public DBTable table { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        public object this[string propertyName]
        {
            get
            {
                if (_properties.ContainsKey(propertyName))
                    return _properties[propertyName];

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
