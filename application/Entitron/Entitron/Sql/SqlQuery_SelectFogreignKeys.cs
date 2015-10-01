using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_SelectFogreignKeys : SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @sql NVARCHAR(MAX), @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SET @sql='SELECT fk.name name, sourceT.name sourceTable, targetT.name targetTable, sourceC.name sourceColumn, targetC.name targetColumn FROM sys.foreign_key_columns fkc " +
                "INNER JOIN sys.foreign_keys fk ON fk.object_id = fkc.constraint_object_id " +
                "INNER JOIN sys.tables sourceT ON sourceT.object_id = fkc.parent_object_id " +
                "INNER JOIN sys.tables targetT ON targetT.object_id = fkc.referenced_object_id " +
                "INNER JOIN sys.columns sourceC ON sourceC.column_id = fkc.parent_column_id AND fk.parent_object_id = sourceC.object_id " +
                "INNER JOIN sys.columns targetC ON targetC.column_id = fkc.referenced_column_id AND fk.referenced_object_id = targetC.object_id " +
                "WHERE sourceT.name = @realTableName';" +
                "exec sp_executesql @sql, N'@realTableName NVARCHAR(50)', @realTableName;",
                parAppName, parTableName
                );

            return base.BaseExecutionWithRead(connection);
        }
    }
}
