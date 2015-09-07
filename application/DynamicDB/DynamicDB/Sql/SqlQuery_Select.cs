using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select : SqlQuery_Selectable
    {
        public List<string> columns { get; set; }
        public string tableName { get; set; }

        public SqlQuery_Select(string applicationName) : base(applicationName)
        {
        }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", tableName);
            _params.Add("columnNames", (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('SELECT ', @columnNames, ' FROM ', @realTableName, ';');" +
                "exec(@sql);";
            
            return base.BaseExecutionWithRead(connection);
        }
    }
}
