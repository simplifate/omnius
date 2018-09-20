using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Column_Default_Drop : SqlQuery_withAppTable
    {
        public DBDefault defaults { get; set; }

        protected override string CreateString()
        {
            return
                $"ALTER TABLE [{realTableName}] DROP CONSTRAINT [{defaults.Name}]";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Drop default on column[{defaults.ColumnName}]";
        }
    }
}
