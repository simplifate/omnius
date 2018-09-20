using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_View_Update : SqlQuery_withApp
    {
        public string viewName { get; set; }
        public string sql { get; set; }

        protected override string CreateString()
        {
            return
                $"ALTER VIEW {RealTableName(application, viewName)} AS {sql};";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{viewName}] Update view";
        }
    }
}
