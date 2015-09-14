using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_PrimaryKeyDrop:SqlQuery_withApp
    {
        public string tableName { get; set; }

        public SqlQuery_PrimaryKeyDrop(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName@{0},@{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, 'DROP CONSTRAINT PK_', @realTableName, ';')"+ 
                "exec(@sql)",
                parAppName, parTableName);

            base.BaseExecution(connection);
        }
    }

}
