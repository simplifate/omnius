using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ConstraintDrop : SqlQuery_withApp
    {
        public string constraintName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                $"ALTER TABLE {realTableName} DROP CONSTRAINT IF EXISTS [{constraintName}];";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop primary key in {0}[{1}]", table.tableName, application.Name);
        }
    }

}
