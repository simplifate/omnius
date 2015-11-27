using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ConstraintDrop : SqlQuery_withApp
    {
        public string constraintName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parConstraintName = safeAddParam("primaryKeyName", constraintName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, ' DROP CONSTRAINT ', @{2} ,';');"+ 
                "exec(@sql);",
                parAppName, parTableName,parConstraintName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop primary key in {0}[{1}]", table.tableName, application.Name);
        }
    }

}
