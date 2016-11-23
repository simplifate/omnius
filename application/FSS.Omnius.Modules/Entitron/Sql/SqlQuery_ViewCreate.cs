using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ViewCreate : SqlQuery_withApp
    {
        public string viewName { get; set; }
        public string sql { get; set; }

        protected override void BaseExecution(MarshalByRefObject connection)
        {

            sqlString = $"CREATE VIEW Entitron_{application.Name}_{viewName} AS {sql} ;";

            base.BaseExecution(connection);
        }
    }
}
