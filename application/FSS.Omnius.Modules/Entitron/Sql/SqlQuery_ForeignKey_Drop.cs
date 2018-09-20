using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_ForeignKey_Drop : SqlQuery_withAppTable
    {
        public string sourceColumn { get; set; }
        public DBTable targetTable { get; set; }
        public string targetcolumn { get; set; }

        protected override string CreateString()
        {
            string realTargetTableName = RealTableName(application, targetTable.Name);
            string foreignKeyName = $"FK_{realTableName}_{sourceColumn}__{realTargetTableName}_{targetcolumn}";

            return
                $"ALTER TABLE {realTableName} DROP CONSTRAINT [{foreignKeyName}]";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Drop foreing key[{sourceColumn}] to [{targetTable.Name}:{targetcolumn}]";
        }
    }
}
