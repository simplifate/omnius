using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Drop : SqlQuery_withApp
    {
        public string columnName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parColumn = safeAddParam("columnName", columnName);

            _sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' DROP COLUMN ', @{2}, ';')" +
                "exec(@sql);", parAppName, parTableName, parColumn);

            base.BaseExecution(connection);
        }

        public override string ToString()
        {
            return string.Format("Drop column {0} in {1}[{2}]", columnName, tableName, applicationName);
        }
    }
}
