using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Table_exists : SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("appName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @meta NVARCHAR(50),@count INT;" +
                "SELECT @meta = DbMetaTables FROM {0} WHERE Name=@{1};" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('SELECT @count = count(*) FROM ', @meta, ' WHERE Name=@tableName;');" +
                "exec sp_executesql @sql, N'@count INT OUTPUT, @tableName NVARCHAR(50)', @count output, @{2};" +
                "SELECT @count count;",
                SqlInitScript.aplicationTableName,
                parAppName,
                parTableName
                );

            List<DBItem> items = base.BaseExecutionWithRead(connection);

            return ((int)items.First()["count"] != 0) ? items : null;
        }
    }
}
