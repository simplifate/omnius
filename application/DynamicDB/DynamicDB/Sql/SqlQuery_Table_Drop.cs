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
        public string tableName { get; set; }

        public SqlQuery_Table_Drop(string ApplicationName) : base(ApplicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parTableName = safeAddParam("tableName", tableName);
            string parAppName = safeAddParam("applicationName", _applicationName);

            _sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@DbTablePrefix NVARCHAR(50),@DbMetaTables NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealNameWithMeta @{0}, @{1}, @realTableName OUTPUT, @DbTablePrefix OUTPUT, @DbMetaTables OUTPUT;" +
                "SET @sql = CONCAT('DROP TABLE ', @realTableName, ';DELETE FROM ', @DbMetaTables, ' WHERE Name = ', @{1});" +
                "exec(@sql);", parAppName,parTableName);

            base.BaseExecution(transaction);
        }
    }
}
