using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Table_Drop : SqlQuery_withApp
    {
        private string _tableName;
        public string tableName { get { return _tableName; } }

        public SqlQuery_Table_Drop(string ApplicationName, string TableName) : base(ApplicationName)
        {
            _tableName = TableName;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", _tableName);

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@DbTablePrefix NVARCHAR(50),@DbMetaTables NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealNameWithMeta @applicationName, @tableName, @realTableName OUTPUT, @DbTablePrefix OUTPUT, @DbMetaTables OUTPUT;" +
                "SET @sql = CONCAT('DROP TABLE ', @realTableName, ';DELETE FROM ', @DbMetaTables, ' WHERE Name = ', @tableName);" +
                "exec(@sql);";

            base.BaseExecution(transaction);
        }
    }
}
