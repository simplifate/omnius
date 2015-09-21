using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_IndexDrop:SqlQuery_withApp
    {
        public string indexName { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parIndexName = safeAddParam("indexName", indexName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('DROP INDEX index_', @{2}, ' ON ', @realTableName, ';')" +
                "exec (@sql)",
                parAppName, parTableName,parIndexName);
            base.BaseExecution(transaction);
        }
    }
}
