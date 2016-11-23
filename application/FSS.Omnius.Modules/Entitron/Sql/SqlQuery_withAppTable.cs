using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_withAppTable : SqlQuery_withApp
    {
        public DBTable table;
        public DBView view;

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");
            if (table == null && view == null)
                throw new ArgumentNullException("table or view");

            base.BaseExecution(connection);
        }
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");
            if (table == null && view == null)
                throw new ArgumentNullException("table or view");

            return base.BaseExecutionWithRead(connection);
        }
    }
}
