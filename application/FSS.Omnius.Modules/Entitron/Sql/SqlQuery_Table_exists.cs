using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_exists : SqlQuery
    {
        public string applicationName;
        public string tableName;

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentNullException("applicationName");
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("tableName");

            string parAppName = safeAddParam("appName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            sqlString = string.Format(
                "SELECT e.Name,e.tableId,a.Name AppName FROM {1} e " +
                "INNER JOIN {0} a ON a.Id=e.ApplicationId " +
                "WHERE e.Name=@{3} AND a.Name=@{2};",
                DB_MasterApplication,
                DB_EntitronMeta,
                parAppName,
                parTableName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
