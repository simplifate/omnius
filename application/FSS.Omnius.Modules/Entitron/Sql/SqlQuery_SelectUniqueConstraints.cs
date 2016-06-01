using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_SelectUniqueConstraints:SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
              
            sqlString =string.Format(
                "DECLARE @sql NVARCHAR(MAX), @realTableName NVARCHAR(100);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SET @sql= 'SELECT i.name uniqueName FROM sys.indexes i INNER JOIN sys.tables t ON t.object_id=i.object_id WHERE i.is_unique_constraint=1 AND t.name = @realTableName ;';"+
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(100)', @realTableName;",
                parAppName,parTableName
                );
            
            return base.BaseExecutionWithRead(connection);
        }
    }
}
