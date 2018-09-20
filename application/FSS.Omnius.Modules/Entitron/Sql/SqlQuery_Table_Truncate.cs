using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Table_Truncate:SqlQuery_withAppTable
    {
        protected override string CreateString()
        {
            return
                $"TRUNCATE TABLE [{realTableName}];";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Truncate table";
        }
    }
}
