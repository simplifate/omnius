using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Select_ColumnList : SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SELECT columns.*, types.name typeName FROM sys.columns columns JOIN sys.types types ON columns.user_type_id = types.user_type_id WHERE object_id = OBJECT_ID(@realTableName)", parAppName, parTableName);
            return base.BaseExecutionWithRead(connection);
        }

        public override string ToString()
        {
            return string.Format("Get coulmn list row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
