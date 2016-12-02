using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_TableTruncate:SqlQuery_withAppTable
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            //string parAppName = safeAddParam("applicationName", application.Name);
            //string parTableName = safeAddParam("tableName", table.tableName);

            sqlString =
                $"TRUNCATE TABLE [{realTableName}];";


                //string.Format(
                //"DECLARE @realTableName NVARCHAR(100), @sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                //"SET @sql=CONCAT('TRUNCATE TABLE ', @realTableName, ';');" +
                //"exec (@sql)",
                //parAppName,parTableName
                //);

            base.BaseExecution(transaction);
        }
    }
}
