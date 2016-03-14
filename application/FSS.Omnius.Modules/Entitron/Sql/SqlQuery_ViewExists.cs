using FSS.Omnius.Modules.CORE;
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

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {

            sqlString = $"SELECT name FROM sys.views WHERE name = 'Entitron_{appName}_{viewName}' ;";

            return base.BaseExecutionWithRead(connection);
        }
    }
}
