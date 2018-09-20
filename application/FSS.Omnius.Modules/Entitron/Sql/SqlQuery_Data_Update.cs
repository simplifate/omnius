using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Data_Update : SqlQuery_withAppTable
    {
        public Dictionary<DBColumn, object> changes { get; set; }
        public int recordId { get; set; }

        protected override string CreateString()
        {
            if (changes == null || changes.Count < 1)
                throw new ArgumentNullException("changes");

            string parRecordId = safeAddParam("recordId", recordId);
            Dictionary<DBColumn, string> parChanges = safeAddParam(changes);
            
            return
                $"UPDATE [{realTableName}] SET {string.Join(", ", parChanges.Select(pair => "[" + pair.Key.Name + "]= @" + pair.Value))} WHERE Id = @{parRecordId};";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Update row";
        }
    }
}
