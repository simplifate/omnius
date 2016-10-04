using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_SelectViews:SqlQuery
    {
        string appName;

        public SqlQuery_SelectViews(string applicationName)
        {
            appName = applicationName;
        }

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            sqlString = $"SELECT name FROM sys.views WHERE name like 'Entitron_{appName}_%';";
            return base.BaseExecutionWithRead(connection);
        }
    }
}
