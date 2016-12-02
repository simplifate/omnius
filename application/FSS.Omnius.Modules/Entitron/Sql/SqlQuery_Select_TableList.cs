using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Select_TableList : SqlQuery_withApp
    {        
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppId = safeAddParam("applicationId", application.Id);

            sqlString =
                $"SELECT Name, tableId FROM {DB_EntitronMeta} WHERE ApplicationId=@{parAppId};";
                
                //string.Format(
                //"SELECT e.Name, e.tableId, a.Name AppName FROM {1} e INNER JOIN {0} a ON e.ApplicationId=a.Id WHERE a.Name=@{2};",
                //DB_MasterApplication,
                //DB_EntitronMeta,
                //parAppName
                //);

            return base.BaseExecutionWithRead(connection);
        }
        
        public override string ToString()
        {
            return string.Format("Get table list in [{0}]", application.Name);
        }
    }
}
