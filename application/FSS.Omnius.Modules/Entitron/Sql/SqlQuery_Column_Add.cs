using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Add : SqlQuery_withApp
    {
        public DBColumn column;
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName= safeAddParam("tableName", table.tableName);
            var parColumn= safeAddParam("columnDefinition", column.getSqlDefinition());

            sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ADD ', @{2});" +
                "exec(@sql);", parAppName,parTableName,parColumn);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add column {0} to {1}[{2}]", column.Name, table.tableName, application.Name);
        }
    }
}
