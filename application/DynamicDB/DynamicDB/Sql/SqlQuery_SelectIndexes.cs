using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicDB.Sql
{
    public class SqlQuery_SelectIndexes:SqlQuery_withApp
    {
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @sql VARCHAR(MAX);" +
                "SET @sql= 'SELECT t.name TableName, i.name IndexName FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name= @{0} ;'" +
                "exec (@sql)",parTableName );

            base.BaseExecution(connection);
        }
    }
}
