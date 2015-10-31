using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_CheckAdd : SqlQuery_withApp
    {
        public Condition_Operators where { get; set; }
        public string checkName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parCheck = safeAddParam("check", string.Join(" AND ",where));
            string parCheckName = safeAddParam("checkName", checkName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT CHK_', {2}, ' CHECK ' , @{3}, ';')" +
                "exec (@sql)",
                parAppName,parTableName, parCheckName,parCheck);


            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add check in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
