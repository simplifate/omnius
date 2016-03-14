using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_SelectDefaultVal:SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @sql NVARCHAR(MAX), @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SET @sql= 'SELECT d.name name, d.definition def FROM sys.default_constraints d " +
                "INNER JOIN sys.tables t ON t.object_id=d.parent_object_id WHERE t.name= @realTableName;'" +
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(50)', @realTableName;",
                parAppName, parTableName);
            return base.BaseExecutionWithRead(connection);
        }
    }
}
