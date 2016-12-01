using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_DefaultAdd:SqlQuery_withAppTable
    {
        public string column { get; set; }
        public object value { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string defaultName = $"DEF_{realTableName}_{column}";

            sqlString =
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT [{defaultName}] DEFAULT '{value}' FOR [{column}];";


            base.BaseExecution(transaction);
        }
    }
}
