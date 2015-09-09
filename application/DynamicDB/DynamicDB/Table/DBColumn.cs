using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DynamicDB
{
    public class DBColumn
    {
        public string Name;
        public SqlDbType type { get; set; }
        public int? maxLength = null;
        public bool canBeNull = true;
        public string additionalOptions = "";

        public virtual string getSqlDefinition()
        {
            return
                string.Format(
                    "{0} {1}{2} {3} {4}",
                    Name,
                    type.ToString(),
                    (maxLength != null) ? "(" + maxLength.ToString() + ")" : "",
                    (canBeNull) ? "NULL" : "NOT NULL",
                    additionalOptions
                    );
        }

        public int? getMaxLength()
        {
            return maxLength;
        }
    }
}
