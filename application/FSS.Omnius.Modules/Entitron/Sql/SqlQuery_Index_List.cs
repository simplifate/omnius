using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Index_List : SqlQuery_withAppTable
    {
        public string columnName { get; set; }

        protected override string CreateString()
        {
            // all table indexes
            if (string.IsNullOrWhiteSpace(columnName))
                return
                    $"SELECT name, is_unique, index_id FROM sys.indexes WHERE name NOT LIKE 'PK_%' AND object_id = object_id('{realTableName}');";

            // column indexes
            else
            {
                string parColumnName = safeAddParam("columnName", columnName);

                return
                    "SELECT i.name, i.is_unique, i.index_id FROM sys.indexes i " +
                    "INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id " +
                    "INNER JOIN sys.all_columns c ON c.object_id = i.object_id AND c.column_id = ic.column_id " +
                    $"WHERE i.name NOT LIKE 'PK_%' AND i.object_id = object_id('{realTableName}') AND c.name = @{parColumnName};";
            }
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] List index on column[{columnName}]";
        }
    }
}
