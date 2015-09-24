using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron
{
    public class DBIndex
    {
        public DBTable table { get; set; }
        public string indexName { get; set; }
        public List<DBColumn> columns { get; set; }
    }
}
