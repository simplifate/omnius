using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_SelectConstrains : SqlQuery_withApp
    {
        public bool isDisable { get; set; }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connectioon)
        {
            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("TableName", table.tableName);

            if (isDisable) //pokud chceme vypnout nekterou constraint musí být nejdříve zaplá a naopak
            {
                sqlString = string.Format(
               "DECLARE @realTableName NVARCHAR(50); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
               "SELECT kc.name name FROM sys.check_constraints kc INNER JOIN sys.tables t ON kc.parent_object_id=t.object_id WHERE kc.is_disabled=0 UNION " +
               "SELECT fk.name name FROM sys.foreign_keys fk INNER JOIN sys.tables t ON fk.parent_object_id=t.object_id WHERE fk.is_disabled=0 AND t.name =  @realTableName ;",
               parAppName,
               parTableName
                );
            }
            else
            {
                sqlString = string.Format(
               "DECLARE @realTableName NVARCHAR(50); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
               "SELECT kc.name name FROM sys.check_constraints kc INNER JOIN sys.tables t ON kc.parent_object_id=t.object_id WHERE kc.is_disabled=1 UNION " +
               "SELECT fk.name name FROM sys.foreign_keys fk INNER JOIN sys.tables t ON fk.parent_object_id=t.object_id WHERE fk.is_disabled=1 AND t.name =  @realTableName ;",
               parAppName,
               parTableName
                );
            }
            

            return base.BaseExecutionWithRead(connectioon);
        }
    }
}
