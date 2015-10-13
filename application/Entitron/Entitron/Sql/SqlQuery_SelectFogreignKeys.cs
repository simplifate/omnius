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
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @metaTable NVARCHAR(50) = (SELECT DbMetaTables meta FROM dbo.Applications WHERE dbo.Applications.Name = @{0});" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('SELECT fk.name name, sourceT.Name sourceTable, sourceC.name sourceColumn, targetT.Name targetTable, targetC.name targetColumn FROM sys.foreign_key_columns fkc " +
                "INNER JOIN sys.foreign_keys fk ON fk.object_id = fkc.constraint_object_id " +
                "INNER JOIN ', @metaTable, ' sourceT ON sourceT.tableId = fkc.parent_object_id " +
                "INNER JOIN ', @metaTable, ' targetT ON targetT.tableId = fkc.referenced_object_id " +
                "INNER JOIN sys.columns sourceC ON sourceC.column_id = fkc.parent_column_id AND fkc.parent_object_id = sourceC.object_id " +
                "INNER JOIN sys.columns targetC ON targetC.column_id = fkc.referenced_column_id AND fkc.referenced_object_id = targetC.object_id " +
                "WHERE sourceT.Name = @tableName OR targetT.Name = @tableName; '); " +
                "exec sp_executesql @sql, N'@tableName NVARCHAR(50)', @{1};",
                parAppName, parTableName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
