using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_View_Drop : SqlQuery_withApp
    {
        public string viewName { get; set; }

        protected override string CreateString()
        {
            return
                $"DROP VIEW {viewName} ;";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{viewName}] Drop view";
        }
    }
}
