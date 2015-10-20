using System;
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

            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("TableName", table.tableName);
            Dictionary<DBColumn, string> values = safeAddParam(rowSelect);
            Dictionary<DBColumn, string> parChanges = safeAddParam(changes);


            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('UPDATE ', @realTableName, ' SET {2} WHERE {3};')" +
                "exec sp_executesql @sql, N'{4}', {5};",
                parAppName, parTableName,
                string.Join(", ", parChanges.Select(pair => pair.Key.Name + "= @" + pair.Value)),
                string.Join(" AND ", values.Select(s => s.Key.Name + "= @" + s.Value)),
                string.Join(", ", _datatypes.Select(s=>"@" + s.Key +" " + s.Value )),
                string.Join(", ", _datatypes.Select(s=>"@" + s.Key))
                );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Update row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
