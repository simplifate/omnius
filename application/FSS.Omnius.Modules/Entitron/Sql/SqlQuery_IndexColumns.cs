using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_IndexColumns:SqlQuery_withApp
    {
        public string indexName { get; set; }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SELECT IndexName = i.Name, ColName = c.Name FROM sys.indexes i " +
                "INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id" +
                "INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id" +
                "INNER JOIN sys.tables t ON i.object_id = t.object_id " +
                "WHERE i.is_primary_key = 0 AND i.name like 'index%' + @{2} AND t.name = 'Entitron_RS_Cars'; ",
                parAppName, parTableName,indexName
                );

            return base.BaseExecutionWithRead(connection);
        }

    }
}
