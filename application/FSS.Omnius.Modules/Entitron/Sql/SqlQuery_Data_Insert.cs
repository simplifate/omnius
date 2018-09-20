using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Data_Insert : SqlQuery_withAppTable
    {
        public Dictionary<DBColumn, object> data { get; set; }

        protected override string CreateString()
        {
            if (data == null || data.Count < 1)
                throw new ArgumentNullException("data");

            Dictionary<DBColumn, string> values = safeAddParam(data);

            return
                $"INSERT INTO [{realTableName}]({string.Join(", ", values.Select(pair => "[" + pair.Key.Name + "]"))}) output Inserted.Id VALUES ({string.Join(", ", values.Select(pair => "@" + pair.Value))});";
        }

        public int GetInsertedId()
        {
            return Convert.ToInt32(ExecuteWithRead().First()["Id"]);
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Insert row";
        }
    }
}
