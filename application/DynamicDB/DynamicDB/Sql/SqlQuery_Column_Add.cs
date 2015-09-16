using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Add : SqlQuery_withApp
    {
        public string tableName;
        public DBColumn column;

        public SqlQuery_Column_Add(string applicationName, string tableName = null, DBColumn column = null) : base(applicationName)
        {
            this.column = column;
            this.tableName = tableName;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName= safeAddParam("tableName", tableName);
            var parColumn= safeAddParam("columnDefinition", column.getSqlDefinition());

            _sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ADD ', @{2});" +
                "exec(@sql);", parAppName,parTableName,parColumn);

            base.BaseExecution(transaction);
        }
    }
}
