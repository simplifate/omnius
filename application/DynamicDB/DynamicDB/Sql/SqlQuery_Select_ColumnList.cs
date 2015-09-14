using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select_ColumnList : SqlQuery_withApp
    {
        public string tableName { get; set; }

        public SqlQuery_Select_ColumnList(string ApplicationName)
            : base(ApplicationName)
        {
        }

        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SELECT columns.*, types.name typeName FROM sys.columns columns JOIN sys.types types ON columns.user_type_id = types.user_type_id WHERE object_id = OBJECT_ID(@realTableName)", parAppName, parTableName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
