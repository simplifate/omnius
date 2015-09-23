using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_PrimaryKeyAdd : SqlQuery_withApp
    {
        public List<string> keyColumns { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (keyColumns == null || keyColumns.Count < 1)
                throw new ArgumentNullException("keyColumn");

            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parColumns = safeAddParam("columns", string.Join(",", keyColumns));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_', @realTableName, ' PRIMARY KEY (', @{2}, ');')" +
                "exec (@sql)",
                parAppName, parTableName, parColumns);
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add primary key to {0}[{1}]", tableName, applicationName);
        }
    }
}
