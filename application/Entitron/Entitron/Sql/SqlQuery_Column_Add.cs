using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Entitron.Sql
{
    class SqlQuery_Column_Add : SqlQuery_withApp
    {
        public DBColumn column;
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName= safeAddParam("tableName", tableName);
            var parColumn= safeAddParam("columnDefinition", column.getSqlDefinition());

            _sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ADD ', @{2});" +
                "exec(@sql);", parAppName,parTableName,parColumn);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add column {0} to {1}[{2}]", column.Name, tableName, applicationName);
        }
    }
}
