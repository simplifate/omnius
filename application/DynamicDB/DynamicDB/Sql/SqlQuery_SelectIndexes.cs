using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicDB.Sql
{
    public class SqlQuery_SelectIndexes:SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @sql NVARCHAR(MAX), @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SET @sql = 'SELECT i.name IndexName FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name=@realTableName AND i.name IS NOT NULL;';" +
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(50)', @realTableName;",
                parAppName, parTableName
                );
        
            return base.BaseExecutionWithRead(connection);
        }
    }
}
