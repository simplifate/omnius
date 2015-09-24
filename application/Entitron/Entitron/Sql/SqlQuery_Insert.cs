using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Insert : SqlQuery_withApp
    {
        public Dictionary<DBColumn, object> data { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (data == null || data.Count < 1)
                throw new ArgumentNullException("data");

            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('INSERT INTO ', @realTableName, '({2}) VALUES({3})');" +
                "exec sp_executesql @sql, N'{4}', {5};",
                parAppName, // 0
                parTableName, // 1
                string.Join(",", data.Select(pair => pair.Key.Name)), // 2
                string.Join(",", data.Select(pair => "@" + pair.Value)), // 3
                string.Join(",", data.Select(pair => pair.Key.getShortSqlDefinition())), // 4
                string.Join(",", data.Select(pair => "@" + pair.Value)) // 5
                );
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Insert row in {0}[{1}]", tableName, applicationName);
        }
    }
}
