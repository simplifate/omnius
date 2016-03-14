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
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            sqlString = "SELECT name FROM sys.views WHERE name like 'Entitron_%';";
            return base.BaseExecutionWithRead(connection);
        }
    }
}
