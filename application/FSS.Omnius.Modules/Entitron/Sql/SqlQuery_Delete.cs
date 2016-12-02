using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Delete : SqlQuery_withAppTable
    {
        public Dictionary<DBColumn,object> rowSelect  { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            Dictionary<DBColumn, string> values = safeAddParam(rowSelect);

            string condition = string.Join(" AND ", values.Select(s => s.Key.Name + "= @" + s.Value));

            sqlString =
                $"DELETE FROM [{realTableName}] OUTPUT DELETED.* WHERE {condition};";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Delete row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
