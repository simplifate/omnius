using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_PrimaryKeyAdd:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public List<string> keyColumns { get; set; }
        public string constraintName { get; set; } 

        public SqlQuery_PrimaryKeyAdd(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (keyColumns == null || keyColumns.Count < 1)
                throw new ArgumentNullException("keyColumn");

            string parAppName = safeAddParam("AppName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parColumns = safeAddParam("columns", string.Join(",", keyColumns));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_', @realTableName, ' PRIMARY KEY (', @{2}, ');')" +
                "exec (@sql)",
                parAppName, parTableName, parColumns);
            
            base.BaseExecution(transaction);
        }
    }
}
