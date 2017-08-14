using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ConstraintDrop : SqlQuery_withAppTable
    {
        public string constraintName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString =
                $"IF(OBJECT_ID('{constraintName}') IS NOT NULL) BEGIN ALTER TABLE [{realTableName}] DROP CONSTRAINT [{constraintName}] END";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop primary key in {0}[{1}]", table.tableName, application.Name);
        }
    }

}
