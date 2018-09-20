using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_List : SqlQuery_withAppTabloid
    {
        protected override string CreateString()
        {
            return
                "SELECT c.column_id, c.name, t.name typeName, c.max_length, c.precision, c.scale, c.is_nullable, ISNULL(i.is_unique, 0) is_unique, SUBSTRING(d.definition, 3, LEN(d.definition) - 4) [default] FROM sys.all_columns c " +
                "INNER JOIN sys.types t ON t.user_type_id = c.user_type_id " +
                $"LEFT JOIN(SELECT ic.index_id, MAX(ic.column_id) column_id FROM sys.index_columns ic WHERE ic.object_id = object_id('{realTabloidName}') GROUP BY ic.index_id HAVING count(*) = 1) icc ON icc.column_id = c.column_id " +
                "LEFT JOIN sys.indexes i ON i.index_id = icc.index_id AND i.object_id = c.object_id " +
                "LEFT JOIN sys.default_constraints d ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id " +
                $"WHERE c.object_id = object_id('{realTabloidName}')";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{tabloid.Name}] List columns";
        }
    }
}
