using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class SqlQuery_Select : SqlQuery_Selectable<SqlQuery_Select>
    {
        public List<string> columns { get; set; }
        
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('SELECT {2} FROM ', @realTableName, ' {3} {4} {5} {6};');" +
                "exec sp_executesql @sql, N'{7}', {8};", 
                parAppName,parTableName,
                (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*",
                _where.ToString(),
                string.Join(" ", _join),
                _group,
                _order,
                string.Join(", ", _datatypes.Select(d => "@" + d.Key + " " + d.Value)),
                string.Join(", ", _datatypes.Select(d => "@" + d.Key))
                );
            
            return base.BaseExecutionWithRead(connection);
        }

        public List<DBItem> ToList()
        {
            return ExecuteWithRead();
        }

        public override string ToString()
        {
            return string.Format("Select row in {0}[{1}]", tableName, applicationName);
        }
    }
}
