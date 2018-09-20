using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Default_List : SqlQuery_withAppTable
    {
        public string columnName { get; set; }

        protected override string CreateString()
        {
            // all table defaults
            if (string.IsNullOrWhiteSpace(columnName))
                return
                    $"SELECT name, SUBSTRING(definition, 3, LEN(df.definition) - 4) value FROM sys.default_constraints WHERE parent_object_id = object_id('{realTableName}')";
            // column defaults
            else
            {
                string parColumnName = safeAddParam("columnName", columnName);

                return
                    $"SELECT df.name name, SUBSTRING(df.definition, 3, LEN(df.definition) - 4) value FROM sys.default_constraints df INNER JOIN sys.all_columns c ON c.object_id = df.parent_object_id AND c.column_id = df.parent_column_id WHERE parent_object_id = object_id('{realTableName}') AND c.name = @{parColumnName}";
            }
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] List defaults";
        }
    }
}
