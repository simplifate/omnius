using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class SqlQuery_Select : SqlQuery_Selectable
    {
        public List<string> columns { get; set; }

        public SqlQuery_Select(string applicationName) : base(applicationName)
        {
        }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", tableName);
            _params.Add("columnNames", (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");
            _params.Add("where", _where);
            _params.Add("join", _join);
            _params.Add("order", _order);
            _params.Add("group", _group);

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('SELECT ', @columnNames, ' FROM ', @realTableName, @join, @where, @order, @group, ';');" +
                "exec(@sql);";
            
            return base.BaseExecutionWithRead(connection);
        }

        public List<DBItem> ToList()
        {
            return ExecuteWithRead();
        }
    }
}
