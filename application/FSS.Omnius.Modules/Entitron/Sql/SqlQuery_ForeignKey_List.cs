using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ForeignKey_List : SqlQuery_withAppTable
    {
        public bool isSource { get; set; }
        public bool isTarget { get; set; }

        protected override string CreateString()
        {
            List<string> where = new List<string>();
            if (isSource)
                where.Add($"parent_object_id = object_id('{realTableName}')");
            if (isTarget)
                where.Add($"referenced_object_id = object_id('{realTableName}')");
            
            return
                "SELECT (SELECT name FROM sys.all_columns WHERE column_id = parent_column_id AND object_id = parent_object_id) sourceColumnName, " +
                "OBJECT_NAME(parent_object_id) sourceTableName," +
                "(SELECT name FROM sys.all_columns WHERE column_id = referenced_column_id AND object_id = referenced_object_id) targetColumnName, " +
                "OBJECT_NAME(referenced_object_id) targetTableName FROM sys.foreign_key_columns " +
                $"WHERE {string.Join(" OR ", where)};";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] List foreing keys";
        }
    }
}
