using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBIndex
    {
        public DBTable table { get; set; }
        public string indexName { get; set; }
        public bool isUnique { get; set; }
        public List<DBColumn> columns { get; set; }

        public override string ToString()
        {
            return indexName;
        }
    }
}
