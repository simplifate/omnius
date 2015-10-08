using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class SqlQuery_ConstraintDisabled:SqlQuery_withApp
    {
        public string constraintName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {

            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("TableName", tableName);
            string parConName = safeAddParam("ConstraintName", constraintName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, ' NOCHECK CONSTRAINT ', @{2}, ' ;');" +
                "exec @sql;",
                parAppName,parTableName,parConName
                );

            base.BaseExecution(transaction);
        }
    }
}
