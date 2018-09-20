using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_View_Exists : SqlQuery_withApp
    {
        public string viewName { get; set; }

        protected override string CreateString()
        {
            return
                $"SELECT name FROM sys.views WHERE name = '{RealTableName(application, viewName)}' ;";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{viewName}] Exists view?";
        }
    }
}
