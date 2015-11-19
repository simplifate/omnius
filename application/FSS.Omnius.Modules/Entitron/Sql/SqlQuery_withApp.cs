using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Sql
{
    public class SqlQuery_withApp : SqlQuery
    {
        public DBApp application;
        public DBTable table;

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");
            if (table == null)
                throw new ArgumentNullException("table");

            base.BaseExecution(connection);
        }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");
            if (table == null)
                throw new ArgumentNullException("table");

            return base.BaseExecutionWithRead(connection);
        }
    }
}
