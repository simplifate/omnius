using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ConstraintDisabled:SqlQuery_withAppTable
    {
        public string constraintName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString =
                $"ALTER TABLE [{realTableName}] NOCHECK CONSTRAINT [{constraintName}];";

            base.BaseExecution(transaction);
        }
    }
}
