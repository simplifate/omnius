using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Select_ColumnList : SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                "SELECT DISTINCT i.is_unique_constraint is_unique, columns.*, types.name typeName FROM sys.columns columns " +
                "JOIN sys.types types ON columns.user_type_id = types.user_type_id " +
                "left join sys.index_columns ic on columns.object_id = ic.object_id and columns.column_id = ic.column_id " +
                "left join sys.indexes i on i.index_id = ic.index_id AND i.object_id = columns.object_id " +
                $"WHERE columns.object_id = OBJECT_ID('{realTableName}')";

            return base.BaseExecutionWithRead(connection);
        }

        public override string ToString()
        {
            return string.Format("Get coulmn list row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
