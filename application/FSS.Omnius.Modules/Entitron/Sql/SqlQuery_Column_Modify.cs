using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Modify : SqlQuery_withApp
    {
        public DBColumn column{ get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string columnDefinition = column.getSqlDefinition();
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            sqlString =
                $"ALTER TABLE [{realTableName}] ALTER COLUMN {columnDefinition};";

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Modify column {0} in {1}[{2}]", column.Name, table.tableName, application.Name);
        }
    }
}
