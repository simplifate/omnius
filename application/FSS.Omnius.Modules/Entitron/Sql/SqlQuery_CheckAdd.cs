using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_CheckAdd : SqlQuery_withAppTable
    {
        public string where { get; set; }
        public string checkName { get; set; }
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string fullCheckName = $"CHK_{application.Name}_{table.tableName}_{checkName}";

            sqlString =
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT {fullCheckName} CHECK {where};";

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add check in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
