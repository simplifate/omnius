using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Sql
{
    class SqlQuery_SelectUniqueConstraints:SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
              
            sqlString =string.Format(
                "DECLARE @sql NVARCHAR(MAX), @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SET @sql= 'SELECT i.name uniqueName FROM sys.indexes i INNER JOIN sys.tables t ON t.object_id=i.object_id WHERE i.is_unique_constraint=1 AND t.name = @realTableName ;';"+
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(50)', @realTableName;",
                parAppName,parTableName
                );
            
            return base.BaseExecutionWithRead(connection);
        }
    }
}
