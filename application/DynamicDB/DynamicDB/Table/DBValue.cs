using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Table
{
    public class DBValue
    {
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public int? MaxLength { get; set; }

        public virtual void setDataType(string valueName, SqlDbType colType, int? colLength)
        {
            Name = valueName;
            Type = colType;
            MaxLength = colLength;
        }

        public virtual string getDataType()
        {
            return string.Format(
                "{0} {1}{2}",
                Name,
                Type.ToString(),
                (MaxLength != null) ? "(" + MaxLength.ToString() + ")" : ""
                );
        }

    }
}
