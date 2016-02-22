using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ViewExists:SqlQuery
    {
        public string viewName { get; set; }
        public string appName { get; set; }

        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", appName);
            string parViewName = safeAddParam("applicationName", viewName);

            sqlString = $"SELECT name FROM sys.views WHERE Entitron_@{parAppName}_@{parViewName};";

            return base.BaseExecutionWithRead(connection);
        }
    }
}
