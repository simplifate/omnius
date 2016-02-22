using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBColumn
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public string type { get; set; }
        public bool isIdentity { get; set; } 
        public bool isPrimary { get; set ; }
        public bool allowColumnLength { get; set; }
        public int? maxLength { get; set; }
        public bool allowPrecisionScale { get; set; }
        public int? precision { get; set; }
        public int? scale { get; set; }
        public bool canBeNull { get; set; }
        public bool isUnique { get; set; }
        public string additionalOptions = "";

        public virtual string getSqlDefinition()
        {
            foreach (string s in DBColumns.getMaxLenghtDataTypes())
            {
                if (type.ToLower() == s)
                {
                    allowColumnLength = true;
                    break;
                }
            }
            if (type == SqlDbType.Decimal.ToString())
            {
                allowPrecisionScale = true;
                if(precision == null) { precision = 38; }
                if(scale == null) { scale = 0; }
            }

            return
                string.Format(
                    "[{0}] {1}{2}{3} {4} {5}{6}",
                    Name,
                    type,
                    (isIdentity)? " IDENTITY(1,1) ":"",
                    (isPrimary==true)?" PRIMARY KEY ":"",
                    (allowColumnLength)
                        ? string.Format("({0})", (maxLength != null || maxLength > 8000) ? maxLength.ToString() : "MAX")
                        : "",
                    (allowPrecisionScale)
                        ? string.Format("({0}, {1})",
                            (precision < 38) ? precision.ToString() : "38",
                            (scale < precision) ? scale.ToString() : precision.ToString()) : "", //avoiding that the value of precision or scale was greater than the range
                    (canBeNull) ? "NULL" : "NOT NULL"
                    );
        }
        public virtual string getShortSqlDefinition()
        {
            return
                string.Format(
                    $"[{0}] {1}{2}",
                    Name,
                    type,
                    (allowColumnLength) ? string.Format("({0})", (maxLength != null) ? maxLength.ToString() : "MAX") : ""
                );
        }

        public override string ToString() => Name;
    }
}
