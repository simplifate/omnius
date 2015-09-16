using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select_TableList : SqlQuery_withApp
    {
        private List<string> _columns;
        public List<string> columns { get; set; }

        public SqlQuery_Select_TableList(string applicationName, List<string> columns = null) : base(applicationName)
        {
            _columns = columns;

            _params.Add("applicationName", _applicationName);
            _params.Add("columnNames", (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");

            _sqlString =
                "DECLARE @tableName NVARCHAR(50) = (SELECT DbMetaTables FROM " + SqlInitScript.aplicationTableName + " WHERE Name = @applicationName);" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('SELECT ', @columnNames, ' FROM ', @tableName, ';');" +
                "exec(@sql);";
        }
    }
}
