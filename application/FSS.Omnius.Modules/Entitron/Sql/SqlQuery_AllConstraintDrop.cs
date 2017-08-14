using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_AllConstraintDrop : SqlQuery_withAppTable
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString =
                $"EXEC dbo.entitronDropConstraints @tableName = '{realTableName}';";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop primary key in {0}[{1}]", table.tableName, application.Name);
        }
    }

}
