using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class SqlQuery_IndexCreate : SqlQuery_withApp
    {
        public List<string> columnsName { get; set; }
        public string indexName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parIndexName = safeAddParam("indexName", indexName);
            string parColumnName = safeAddParam("columnName", string.Join(", ",columnsName));

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('CREATE INDEX index_', @{2} , ' ON ', @realTableName, '(', @{3}, ');')" +
                "exec (@sql)",
                parAppName, parTableName,parIndexName, parColumnName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add index {0} in {1}[{2}]", indexName, table.tableName, application.Name);
        }
    }
}
