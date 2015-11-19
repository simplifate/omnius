using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Sql
{
    class SqlQuery_Select_TableList : SqlQuery
    {
        public string ApplicationName;
        
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (ApplicationName == null)
                throw new ArgumentNullException("ApplicationName");

            string parAppName = safeAddParam("applicationName", ApplicationName);

            sqlString = string.Format(
                "SELECT e.Name, e.tableId, a.Name AppName FROM {1} e INNER JOIN {0} a ON e.ApplicationId=a.Id WHERE a.Name=@{2};",
                DB_MasterApplication,
                DB_EntitronMeta,
                parAppName
                );

            return base.BaseExecutionWithRead(connection);
        }
        
        public override string ToString()
        {
            return string.Format("Get table list in [{0}]", ApplicationName);
        }
    }
}
