using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select_TableList : SqlQuery_withApp
    {
        public List<string> columns { get; set; }

        public SqlQuery_Select_TableList(string applicationName) : base(applicationName)
        {
        }

        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parColumnName = safeAddParam("columnNames",
                (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");

            _sqlString =string.Format(
                "DECLARE @tableName NVARCHAR(50) = (SELECT DbMetaTables FROM dbo.Applications WHERE Name = @{0});" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('SELECT ', @{1}, ' FROM ', @tableName, ';');" +
                "exec(@sql);", parAppName,parColumnName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
