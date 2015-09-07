using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select_ColumnList : SqlQuery_withApp
    {
        private string _tableName;
        public string tableName { get { return _tableName; } }

        public SqlQuery_Select_ColumnList(string ApplicationName, string tableName) : base(ApplicationName)
        {
            _tableName = tableName;

            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", _tableName);

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SELECT* FROM sys.columns WHERE object_id = OBJECT_ID(@realTableName);";
        }
    }
}
