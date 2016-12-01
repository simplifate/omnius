using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_exists : SqlQuery_withApp
    {
        public string tableName;

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("tableName");

            string parAppId = safeAddParam("appName", application.Id);
            string parTableName = safeAddParam("tableName", tableName);

            sqlString = $"SELECT Name, tableId FROM {DB_EntitronMeta} WHERE Name=@{parTableName} AND ApplicationId=@{parAppId};";
                
                //string.Format(
                //"SELECT e.Name,e.tableId,a.Name AppName FROM {1} e " +
                //"INNER JOIN {0} a ON a.Id=e.ApplicationId " +
                //"WHERE e.Name=@{3} AND a.Name=@{2};",
                //DB_MasterApplication,
                //DB_EntitronMeta,
                //parAppName,
                //parTableName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
