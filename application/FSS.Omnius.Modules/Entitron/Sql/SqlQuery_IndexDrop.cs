using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_IndexDrop : SqlQuery_withAppTable
    {
        public string indexName { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parIndexName = safeAddParam("indexName", indexName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(100), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('DROP INDEX IF EXISTS ', @{2}, ' ON ', @realTableName, ';')" +
                "exec (@sql)",
                parAppName, parTableName,parIndexName);
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop index {0} in {1}[{2}]", indexName, table.tableName, application.Name);
        }
    }
}
