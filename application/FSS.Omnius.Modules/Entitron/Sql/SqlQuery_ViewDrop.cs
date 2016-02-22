using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ViewDrop:SqlQuery
    {
        public string viewName { get; set; }
        public string appName { get; set; }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", appName);
            string parViewName = safeAddParam("applicationName", viewName);

            sqlString= $"DROP VIEW IF EXISTS Entitron_@{parAppName}_@{parViewName} ;";

            base.BaseExecution(connection);
        }
    }
}
