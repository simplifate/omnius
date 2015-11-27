using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Rename : SqlQuery_withApp
    {
        public string originColumnName { get; set; }
        public string newColumnName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parOriginName = safeAddParam("originColumnName", originColumnName);
            string parNewName = safeAddParam("newColumnName", newColumnName);

            sqlString =
                string.Format(
                "DECLARE @realTableName NVARCHAR(50),@fullOriginName NVARCHAR(100);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @fullOriginName = CONCAT(@realTableName, '.', @{2});" +
                "exec sp_RENAME @fullOriginName, @{3}, 'COLUMN';",
                parAppName, parTableName, parOriginName, parNewName);

            base.BaseExecution(connection);
        }

        public override string ToString()
        {
            return string.Format("Rename column {0} in {1}[{2}]", originColumnName, table.tableName, application.Name);
        }
    }
}
