using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ForeignKey_Add : SqlQuery_withAppTable
    {
        public string sourceColumn { get; set; }
        public DBTable targetTable { get; set; }
        public string targetcolumn { get; set; }
        public string onDelete { get; set; }
        public string onUpdate { get; set; }

        protected override string CreateString()
        {
            string realTargetTableName = RealTableName(application, targetTable.Name);
            string foreignKeyName = $"FK_{realTableName}_{sourceColumn}__{realTargetTableName}_{targetcolumn}";

            string update = (onUpdate == "cascade")
                ? " ON UPDATE CASCADE"
                : (onUpdate == "null")
                    ? " ON UPDATE SET NULL"
                    : (onUpdate == "default") ? " ON UPDATE SET DEFAULT" : "";
            string delete = (onDelete == "cascade")
                ? " ON DELETE CASCADE"
                : (onDelete == "null")
                    ? " ON DELETE SET NULL"
                    : (onDelete == "default") ? " ON DELETE SET DEFAULT" : "";

            return
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT [{foreignKeyName}] FOREIGN KEY ([{sourceColumn}]) REFERENCES [{realTargetTableName}]([{targetcolumn}]) {delete} {update};";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Add foreing key[{sourceColumn}] to [{targetTable.Name}:{targetcolumn}]";
        }
    }
}
