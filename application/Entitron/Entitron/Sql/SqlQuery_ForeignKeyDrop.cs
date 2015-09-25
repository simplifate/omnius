using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_ForeignKeyDrop : SqlQuery_withApp
    {
        public string foreignKeyName { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parForeignKeyName = safeAddParam("foreignKeyName", foreignKeyName);


            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName@{0},@{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, 'DROP CONSTRAINT ', @{2}, ';')" +
                "exec(@sql)",
                parAppName, parTableName,foreignKeyName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add Foreign key in {0}[{1}]", tableName, applicationName);
        }
    }
}
