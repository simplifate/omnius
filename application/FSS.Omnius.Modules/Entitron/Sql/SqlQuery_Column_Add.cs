using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Add : SqlQuery_withApp
    {
        public DBColumn column;
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string columnDefinition = column.getSqlDefinition();
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                $"ALTER TABLE [{realTableName}] ADD {columnDefinition};";

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add column {0} to {1}[{2}]", column.Name, table.tableName, application.Name);
        }
    }
}
