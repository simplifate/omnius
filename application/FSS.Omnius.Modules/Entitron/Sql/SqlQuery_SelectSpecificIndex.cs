using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_SelectSpecificIndex:SqlQuery_withApp
    {
        public string indexName { get; set; }
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parIndexName = safeAddParam("indexName", indexName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50);" +
                "exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SELECT i.name IndexName, i.is_unique isUnique FROM sys.indexes i " +
                "INNER JOIN sys.tables t ON i.object_id = t.object_id " +
                "WHERE i.is_primary_key=0 AND i.name like 'index%' + @{2} AND t.name=@realTableName;",
                parAppName, parTableName, parIndexName
                );

            return base.BaseExecutionWithRead(connection);
        }

    }
}
