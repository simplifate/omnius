using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_IndexCreate:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public List<string> columnName { get; set; } 

        public SqlQuery_IndexCreate(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parColumnName = safeAddParam("columnName", string.Join(", ",columnName));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('CREATE INDEX index_', @realTableName, ' ON ', @realTableName, '(', @{2}, ');')" +
                "exec (@sql)",
                parAppName, parTableName, parColumnName);

            base.BaseExecution(transaction);
        }
    }
}
