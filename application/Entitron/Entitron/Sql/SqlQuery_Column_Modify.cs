using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Column_Modify : SqlQuery_withApp
    {
        public DBColumn column{ get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            var parColumn = safeAddParam("columnDefinition", column.getSqlDefinition());

            sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ALTER COLUMN ', @{2});" +
                "exec(@sql);", parAppName, parTableName, parColumn);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Modify column {0} in {1}[{2}]", column.Name, table.tableName, application.Name);
        }
    }
}
