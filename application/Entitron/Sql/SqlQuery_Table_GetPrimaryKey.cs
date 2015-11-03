using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_Table_GetPrimaryKey : SqlQuery_withApp
    {
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTabName = safeAddParam("TableName", table.tableName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SELECT column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = @realTableName",
                parAppName, parTabName);

            return base.BaseExecutionWithRead(connection);
        }
        
        public override string ToString()
        {
            return string.Format("Get primary keys in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
