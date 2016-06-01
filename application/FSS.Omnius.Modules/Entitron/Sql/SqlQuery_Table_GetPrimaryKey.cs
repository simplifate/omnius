using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_GetPrimaryKey : SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTabName = safeAddParam("TableName", table.tableName);

            sqlString =
                $"DECLARE @realTableName NVARCHAR(100);exec getTableRealName @{parAppName}, @{parTabName}, @realTableName OUTPUT;" +
                "SELECT column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 AND table_name = @realTableName";

            return base.BaseExecutionWithRead(connection);
        }
        
        public override string ToString()
        {
            return string.Format("Get primary keys in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
