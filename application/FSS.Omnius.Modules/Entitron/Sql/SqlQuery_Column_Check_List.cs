using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Column_Check_List : SqlQuery_withAppTable
    {
        public string columnName { get; set; }

        protected override string CreateString()
        {
            // all table checks
            if (string.IsNullOrWhiteSpace(columnName))
                return
                    $"SELECT name, definition FROM sys.check_constraints WHERE parent_object_id = object_id('{realTableName}')";
            // column checks
            else
            {
                string parColumnName = safeAddParam("columnName", columnName);

                return
                    $"SELECT ch.name, ch.definition FROM sys.check_constraints ch INNER JOIN sys.all_columns c ON c.object_id = ch.parent_object_id AND c.column_id = ch.parent_column_id WHERE parent_object_id = object_id('{realTableName}') AND c.name = @{parColumnName}";
            }
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] List check";
        }
    }
}
