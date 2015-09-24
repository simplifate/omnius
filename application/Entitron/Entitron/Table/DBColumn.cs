using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Entitron
{
    public class DBColumn
    {
        public string Name { get; set; }
        public string type { get; set; }
        public bool allowColumnLength { get; set; }
        public int? maxLength { get; set; }
        public bool canBeNull { get; set; }
        public bool isPrimaryKey { get; set; }
        public bool isUnique { get; set; }
        public string additionalOptions = "";

        public virtual string getSqlDefinition()
        {
            return
                string.Format(
                    "{0} {1}{2} {3} {4} {5} {6}",
                    Name,
                    type,
                    (allowColumnLength) ? string.Format("({0})", (maxLength != null) ? maxLength.ToString() : "MAX") : "",
                    (canBeNull==true) ? "NULL" : "NOT NULL",
                    (isUnique == true) ? "UNIQUE" : "",
                    (isPrimaryKey == true) ? "PRIMARY KEY" : "",
                    additionalOptions
                    );
        }
        public virtual string getShortSqlDefinition()
        {
            return
                string.Format(
                    "{0} {1}{2}",
                    Name,
                    type,
                    (allowColumnLength) ? string.Format("({0})", (maxLength != null) ? maxLength.ToString() : "MAX") : ""
                );
        }
    }
}
