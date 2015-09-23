using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_CheckAdd : SqlQuery_Selectable
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parCheck = safeAddParam("check", string.Join(" AND ",_where));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, 'ADD CONSTRAINT CHK_',@realTableName, ' ' , @{2}, ';')" +
                "exec (@sql)",
                parAppName,parTableName,parCheck);


            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add check in {0}[{1}]", tableName, applicationName);
        }
    }
}
