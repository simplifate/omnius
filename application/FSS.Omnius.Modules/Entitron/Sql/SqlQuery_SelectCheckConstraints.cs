using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_SelectCheckConstraints:SqlQuery_withAppTable
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(100), @sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= 'SELECT ch.name name FROM sys.check_constraints ch " +
                "INNER JOIN sys.tables t ON t.object_id=ch.parent_object_id " +
                "WHERE t.name = @realTableName;';" +
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(100)', @realTableName;",
                parAppName, parTableName
                );

            return base.BaseExecutionWithRead(connection);
        }
    }
}
