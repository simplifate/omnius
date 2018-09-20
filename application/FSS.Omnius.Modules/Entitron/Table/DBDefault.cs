using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron
{
    public class DBDefault
    {
        public static DBDefault Create(DBTabloid tabloid, string columnName)
        {
            return new DBDefault { Tabloid = tabloid, ColumnName = columnName };
        }
        public static DBDefault Create(DBTabloid tabloid, string columnName, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return new DBDefault { Tabloid = tabloid, ColumnName = columnName, Value = value };
        }

        private DBDefault() { }

        public DBTabloid Tabloid { get; set; }
        public string ColumnName { get; set; }
        public string Value { get; set; }

        public string Name
        {
            get
            {
                return $"DEF_Entitron_{Tabloid.Application.Name}_{Tabloid.Name}_{ColumnName}";
            }
        }

        public string getSqlDefinition()
        {
            return $" CONSTRAINT [{Name}] DEFAULT '{Value}'";
        }
    }
}
