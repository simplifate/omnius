using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_SelectPrimaryKeyName:SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTabName = safeAddParam("TableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(100);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SELECT i.name FROM sys.indexes i inner join sys.tables t on t.object_id=i.object_id  where i.is_primary_key=1 and t.name = @realTableName",
                parAppName, parTabName);


            return base.BaseExecutionWithRead(connection);
        }
    }
}
