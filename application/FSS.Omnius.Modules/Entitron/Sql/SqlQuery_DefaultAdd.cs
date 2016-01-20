using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_DefaultAdd:SqlQuery_withApp
    {
        public string column { get; set; }
        public object value { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parColName = safeAddParam("columnName", column);
            string parValName = safeAddParam("value", value);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT DEF_', @realTableName , @{2},' DEFAULT ''', @{3},''' FOR ', @{2},';');" +
                "exec (@sql);",
                parAppName,parTableName,parColName,parValName
                );
            base.BaseExecution(transaction);
        }
    }
}
