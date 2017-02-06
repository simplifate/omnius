using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_withApp : SqlQuery
    {
        public Application application;

        public override string connectionString
        {
            get
            {
                return application.connectionString_data;
            }
        }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");

            base.BaseExecution(connection);
        }
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                throw new ArgumentNullException("application");

            return base.BaseExecutionWithRead(connection);
        }
    }
}
