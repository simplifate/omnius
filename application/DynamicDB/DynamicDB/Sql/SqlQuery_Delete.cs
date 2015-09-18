using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Delete:SqlQuery_withApp
    {
        public Dictionary<DBColumn, object> columnValueCondition { get; set; }

        public SqlQuery_Delete(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("TableName", tableName);
            var parConditions = safeAddParam(columnValueCondition);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('DELETE FROM ', @realTableName, ' WHERE {2} ;')"+
                "exec sp_executesql @sql, N'{3} ,{4}" 
                , parAppName,parTableName,
            string.Join(" AND ", parConditions.Select(pair => pair.Key.Name + "=@" + pair.Key.Name)),
            string.Join(",", parConditions.Select(pair => pair.Key.getShortSqlDefinition())),
            string.Join(",", parConditions.Select(pair => string.Format("@{0}=@{1}", pair.Key.Name, pair.Value)))
            );

            base.BaseExecution(transaction);
        }
    }
}
