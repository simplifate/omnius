using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_SelectConstrains : SqlQuery_withApp
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("TableName", table.tableName);

            sqlString = string.Format(
               "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
               "SET @sql= CONCAT('SELECT kc.name name FROM sys.key_constraints kc INNER JOIN sys.tables t ON kc.object_id=t.object_id UNION " +
               "SELECT fk.name name FROM sys.foreign_keys fk INNER JOIN sys.tables t ON fk.object_id=t.object_id UNION " +
               "SELECT i.name name FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id=t.object_id " +
               "WHERE t.name = ', @realTableName, ';');",
               parAppName,
               parTableName
                );

            base.BaseExecution(transaction);
        }
    }
}
