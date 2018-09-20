using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Index_Drop : SqlQuery_withAppTable
    {
        public List<string> columnsName { get; set; }
        public bool isUniqueIndex { get; set; }
        private string indexName => $"{(isUniqueIndex ? "UN" : "IX")}_{realTableName}_{string.Join("_", columnsName)}";

        protected override string CreateString()
        {
            return
                $"DROP INDEX {indexName} ON [{realTableName}];";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Drop index[{(isUniqueIndex ? "UN" : "IN")} {string.Join(",", columnsName)}]";
        }
    }
}
