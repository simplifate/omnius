﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Update : SqlQuery_withApp
    {
        public Dictionary<DBColumn, object> changes { get; set; }
        public Dictionary<DBColumn, object> rowSelect { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (changes == null || changes.Count < 1)
                throw new ArgumentNullException("changes");

            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("TableName", tableName);
            Dictionary<DBColumn, string> values = safeAddParam(rowSelect);


            var parChanges = safeAddParam(changes);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('UPDATE ', @realTableName, ' SET {2} WHERE {3};')" +
                "exec sp_executesql @sql, N'{4}{5}', {6},{7};",
                parAppName, parTableName,
                string.Join(",", parChanges.Select(pair => pair.Key.Name + "=@" + pair.Value)),
                string.Join(" AND ", values.Select(s => s.Key.Name + "= @" + s.Value)),
                string.Join(",", parChanges.Select(pair =>"@" + pair.Key.getShortSqlDefinition())),
                string.Join(", ", values.Select(s => "@" + s.Key.getShortSqlDefinition())),
                string.Join(",", parChanges.Select(pair => "@" + pair.Value)),
                string.Join(", ", values.Select(s => "@" + s.Value))
                );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Update row in {0}[{1}]", tableName, applicationName);
        }
    }
}
