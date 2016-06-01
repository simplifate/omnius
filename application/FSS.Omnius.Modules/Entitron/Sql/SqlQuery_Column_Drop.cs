using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Drop : SqlQuery_withApp
    {
        public string columnName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject connection)
        {
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                $"ALTER TABLE {realTableName} DROP COLUMN IF EXISTS [{columnName}];";

            base.BaseExecution(connection);
        }

        public override string ToString()
        {
            return string.Format("Drop column {0} in {1}[{2}]", columnName, table.tableName, application.Name);
        }
    }
}
