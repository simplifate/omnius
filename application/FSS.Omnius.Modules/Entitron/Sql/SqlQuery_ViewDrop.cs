using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ViewDrop : SqlQuery_withApp
    {
        public string viewName { get; set; }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            sqlString= $"DROP VIEW IF EXISTS {viewName} ;";

            base.BaseExecution(connection);
        }
    }
}
