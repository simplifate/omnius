using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_View_List : SqlQuery_withApp
    {
        protected override string CreateString()
        {
            return
                $"SELECT name FROM sys.views WHERE name like 'Entitron_{application.Name}_%';";
        }

        public override string ToString()
        {
            return $"[{application.Name}] List views";
        }
    }
}
