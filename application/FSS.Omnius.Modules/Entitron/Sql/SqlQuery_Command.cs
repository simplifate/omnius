using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Command : SqlQuery_withoutApp
    {
        public string Sql { get; set; }

        public SqlQuery_Command(string connection) : base(connection)
        {
        }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            sqlString = Sql;

            base.BaseExecution(connection);
        }
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            sqlString = Sql;

            return base.BaseExecutionWithRead(connection);
        }
    }
}
