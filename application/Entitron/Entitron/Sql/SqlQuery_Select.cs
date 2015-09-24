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
            string parColumnName = safeAddParam("columnNames", (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");
            string parWhere = safeAddParam("where", _where.ToString());
            string parJoin = safeAddParam("join", string.Join(" ", _join));
            string parOrder = safeAddParam("order", _order);
            string parGroup = safeAddParam("group", _group);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('SELECT ', @{2}, ' FROM ', @realTableName, @{3}, @{4}, @{5}, @{6}, ';');" +
                "exec(@sql);", parAppName,parTableName,parColumnName, parJoin, parWhere,parGroup, parOrder);
            
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
