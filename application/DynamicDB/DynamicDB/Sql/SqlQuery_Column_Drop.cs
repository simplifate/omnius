using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Drop : SqlQuery_withApp
    {
        public string tableName { get; set; }
        public string columnName { get; set; }

        public SqlQuery_Column_Drop(string ApplicationName) : base(ApplicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", tableName);
            _params.Add("columnName", columnName);

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' DROP COLUMN ', @columnName, ';')" +
                "exec(@sql);";

            base.BaseExecution(connection);
        }
    }
}
