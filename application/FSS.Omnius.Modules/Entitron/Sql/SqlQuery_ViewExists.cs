using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ViewExists : SqlQuery_withApp
    {
        public string viewName { get; set; }

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string realName = $"Entitron_{(application.Id == SharedTables.AppId ? SharedTables.Prefix : application.Name)}_{viewName}";

            sqlString = $"SELECT name FROM sys.views WHERE name = '{realName}' ;";

            return base.BaseExecutionWithRead(connection);
        }
    }
}
