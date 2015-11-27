using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Delete : SqlQuery_withApp
    {
        public Dictionary<DBColumn,object> rowSelect  { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("TableName", table.tableName);
            Dictionary<DBColumn, string> values = safeAddParam(rowSelect);
           
            
            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('DELETE FROM ', @realTableName, ' WHERE {2} ;')"+
                "exec sp_executesql @sql, N'{3}', {4};" 
                , parAppName,parTableName,
                string.Join(" AND ", values.Select(s=>s.Key.Name + "= @" + s.Value)),
                string.Join(", ", _datatypes.Select(s => "@" + s.Key + " " + s.Value)),
                string.Join(", ", _datatypes.Select(s => "@" + s.Key))
                
            );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Delete row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
