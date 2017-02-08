using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_SelectViews : SqlQuery_withApp
    {
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string realNameLike = $"Entitron_{(application.Id == SharedTables.AppId ? SharedTables.Prefix : application.Name)}_%";
            sqlString = $"SELECT name FROM sys.views WHERE name like '{realNameLike}';";
            return base.BaseExecutionWithRead(connection);
        }
    }
}
