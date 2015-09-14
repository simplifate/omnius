using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Update : SqlQuery_withApp
    {
        public string tableName;
        public Dictionary<DBColumn, object> changes { get; set; }
        public Dictionary<DBColumn, object> rowSelect { get; set; }

        public SqlQuery_Update(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (changes == null || changes.Count < 1)
                throw new ArgumentNullException("changes");
            if (rowSelect == null || rowSelect.Count < 1)
                throw new ArgumentNullException("rowSelect");

            string parAppName = safeAddParam("AppName", _applicationName);
            string parTableName = safeAddParam("TableName", tableName);

            var parChanges = safeAddParam(changes);
            var parRowSelect = safeAddParam(rowSelect);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('UPDATE ', @realTableName, ' SET {2} WHERE {3};')" +
                "exec sp_executesql @sql, N'{4},{5}', {6},{7};",
                parAppName, parTableName,
                string.Join(",", parChanges.Select(pair => pair.Key.Name + "=@" + pair.Value)),
                string.Join(",", parRowSelect.Select(pair => pair.Key.Name + "=@" + pair.Value)),

                string.Join(",", parChanges.Select(pair => pair.Key.getShortSqlDefinition())),
                string.Join(",", parRowSelect.Select(pair => pair.Key.getShortSqlDefinition())),
                
                string.Join(",", parChanges.Select(pair => "@" + pair.Value)),
                string.Join(",", parRowSelect.Select(pair => "@" + pair.Value))
                );

            base.BaseExecution(transaction);
        }
    }
}
