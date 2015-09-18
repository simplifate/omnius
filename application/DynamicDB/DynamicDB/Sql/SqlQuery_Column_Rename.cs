using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Rename:SqlQuery_withApp
    {
        public string originColumnName { get; set; }
        public string newColumnName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            string parOriginName = safeAddParam("originColumnName", originColumnName);
            string parNewName = safeAddParam("newColumnName", newColumnName);

            _sqlString =
                string.Format(
                "DECLARE @realTableName NVARCHAR(50),@fullOriginName NVARCHAR(100);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @fullOriginName = CONCAT(@realTableName, '.', @{2});" +
                "exec sp_RENAME @fullOriginName, @{3}, 'COLUMN';",
                parAppName, tableName, parOriginName, parNewName);

            base.BaseExecution(connection);
        }
    }
}
