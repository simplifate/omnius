using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Index_Create : SqlQuery_withAppTable
    {
        public List<string> columnsName { get; set; }
        public bool isUniqueIndex { get; set; }
        private string indexName => $"{(isUniqueIndex ? "UN" : "IX")}_{realTableName}_{string.Join("_", columnsName)}";
        
        protected override string CreateString()
        {
            string unique = (isUniqueIndex) ? "UNIQUE " : "";

            return
                $"CREATE {unique}INDEX [{indexName}] ON [{realTableName}]([{string.Join("], [", columnsName)}])";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Add index[{(isUniqueIndex ? "UN" : "IN")} {string.Join(",", columnsName)}]";
        }
    }
}
