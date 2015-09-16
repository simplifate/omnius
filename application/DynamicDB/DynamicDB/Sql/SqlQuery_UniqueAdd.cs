using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_UniqueAdd:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public List<string> keyColumns { get; set; } 

        public SqlQuery_UniqueAdd(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parColumns = safeAddParam("columns", string.Join(",", keyColumns));

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT UN_', @realTableName, ' UNIQUE (', @{2}, ');')" +
                "exec (@sql)",
                parAppName, parTableName, parColumns);

            base.BaseExecution(transaction);
        }
    }
}
