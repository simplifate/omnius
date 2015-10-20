using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_SelectFogreignKeys : SqlQuery_withApp
    {
        public bool? isForDrop { get; set; }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            
            if (isForDrop==true)
            {
                 sqlString = string.Format(
                "DECLARE @_appId INT=(SELECT Id FROM {0} WHERE Name= @{2});" +
                "SELECT fk.name name,sourceT.Name sourceTable,sourceC.name sourceColumn,targetT.Name targetTable,targetC.name targetColumn FROM sys.foreign_key_columns fkc " +
                "INNER JOIN sys.foreign_keys fk ON fk.object_id=fkc.constraint_object_id " +
                "INNER JOIN {1} sourceT ON sourceT.tableId=fkc.parent_object_id " +
                "INNER JOIN {1} targetT ON targetT.tableId=fkc.referenced_object_id " +
                "INNER JOIN sys.columns sourceC ON sourceC.column_id=fkc.parent_column_id AND fkc.parent_object_id=sourceC.object_id " +
                "INNER JOIN sys.columns targetC ON targetC.column_id=fkc.referenced_column_id AND fkc.referenced_object_id=targetC.object_id " + 
                "WHERE sourceT.Name=@{3} AND sourceT.ApplicationId=@_appId;",
                DB_MasterApplication,
                DB_EntitronMeta,
                parAppName, parTableName);
            }
            else
            {
                sqlString = string.Format(
               "DECLARE @_appId INT=(SELECT Id FROM {0} WHERE Name= @{2});" +
               "SELECT fk.name name,sourceT.Name sourceTable,sourceC.name sourceColumn,targetT.Name targetTable,targetC.name targetColumn FROM sys.foreign_key_columns fkc " +
               "INNER JOIN sys.foreign_keys fk ON fk.object_id=fkc.constraint_object_id " +
               "INNER JOIN {1} sourceT ON sourceT.tableId=fkc.parent_object_id " +
               "INNER JOIN {1} targetT ON targetT.tableId=fkc.referenced_object_id " +
               "INNER JOIN sys.columns sourceC ON sourceC.column_id=fkc.parent_column_id AND fkc.parent_object_id=sourceC.object_id " +
               "INNER JOIN sys.columns targetC ON targetC.column_id=fkc.referenced_column_id AND fkc.referenced_object_id=targetC.object_id " +
               "WHERE (sourceT.Name= @{3} AND sourceT.ApplicationId=@_appId) OR (targetT.Name= @{3} AND targetT.ApplicationId=@_appId);",
               DB_MasterApplication,
               DB_EntitronMeta,
               parAppName, parTableName);
            }
           

            return base.BaseExecutionWithRead(connection);
        }
    }
}
