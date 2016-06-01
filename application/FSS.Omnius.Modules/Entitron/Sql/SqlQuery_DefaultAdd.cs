using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_DefaultAdd:SqlQuery_withApp
    {
        public string column { get; set; }
        public object value { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";
            string defaultName = $"DEF_{realTableName}_{column}";
            string parValName = safeAddParam("value", value);

            sqlString =
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT [{defaultName}] DEFAULT @{parValName} FOR [{column}];";
            
            base.BaseExecution(transaction);
        }
    }
}
