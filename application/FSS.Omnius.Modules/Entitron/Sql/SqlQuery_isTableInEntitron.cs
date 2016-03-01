using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_isTableInEntitron:SqlQuery
    {

        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {

            sqlString = "SELECT Distinct TABLE_NAME name FROM INFORMATION_SCHEMA.TABLES";

            return base.BaseExecutionWithRead(connection);
        }
    }
}
