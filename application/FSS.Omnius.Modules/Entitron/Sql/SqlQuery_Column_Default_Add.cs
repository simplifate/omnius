using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Column_Default_Add : SqlQuery_withAppTable
    {
        public DBDefault defaults { get; set; }

        protected override string CreateString()
        {
            return
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT [{defaults.Name}] DEFAULT '{defaults.Value}' FOR [{defaults.ColumnName}];";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Add default value '{defaults.Value}' to column[{defaults.ColumnName}]";
        }
    }
}
