using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Index_ListColumns : SqlQuery_withAppTable
    {
        public int indexId { get; set; }

        protected override string CreateString()
        {
            string parIndexId = safeAddParam("indexId", indexId);

            return
                "SELECT c.name FROM sys.index_columns ic " +
                "JOIN sys.all_columns c ON c.column_id = ic.column_id AND c.object_id = ic.object_id " +
                $"WHERE index_id = @indexId AND ic.object_id = object_id('{realTableName}') " +
                "ORDER BY ic.key_ordinal";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] List index[{indexId}] columns";
        }
    }
}
