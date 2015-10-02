using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron
{
    public class DBForeignKey
    {
        public string name { get; set; }

        public string sourceTable { get; set; }
        public string targetTable { get; set; }
        public string sourceColumn { get; set; }
        public string targetColumn { get; set; }
        public string onDelete { get; set; }
        public string onUpdate { get; set; }
    }
}
