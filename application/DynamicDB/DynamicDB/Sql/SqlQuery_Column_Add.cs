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
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", tableName);
            _params.Add("columnDefinition", column.getSqlDefinition());

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ADD ', @columnDefinition);" +
                "exec(@sql);";

            base.BaseExecution(transaction);
        }
    }
}
