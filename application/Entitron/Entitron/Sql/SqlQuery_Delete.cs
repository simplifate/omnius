using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Delete : SqlQuery_withApp
    {
        public Dictionary<DBColumn,object> rowSelect  { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", applicationName);
            string parTableName = safeAddParam("TableName", tableName);
            Dictionary<DBColumn, string> values = safeAddParam(rowSelect);
           
            
            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('DELETE FROM ', @realTableName, ' WHERE {2} ;')"+
                "exec sp_executesql @sql, N'{3}', {4};" 
                , parAppName,parTableName,
                string.Join(" AND ", values.Select(s=>s.Key.Name + "= @" + s.Value)),
                string.Join(", ", values.Select(s=>"@" + s.Key.getShortSqlDefinition())),
                string.Join(", ", values.Select(s=>"@" + s.Value))
                
            );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Delete row in {0}[{1}]", tableName, applicationName);
        }
    }
}
