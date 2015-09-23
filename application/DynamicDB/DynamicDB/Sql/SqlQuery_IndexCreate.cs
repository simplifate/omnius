using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class SqlQuery_IndexCreate : SqlQuery_withApp
    {
        public List<string> columnsName { get; set; }
        public string indexName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parIndexName = safeAddParam("indexName", indexName);
            string parColumnName = safeAddParam("columnName", string.Join(", ",columnsName));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('CREATE INDEX index_', @{2} , ' ON ', @realTableName, '(', @{3}, ');')" +
                "exec (@sql)",
                parAppName, parTableName,parIndexName, parColumnName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add index {0} in {1}[{2}]", indexName, tableName, applicationName);
        }
    }
}
