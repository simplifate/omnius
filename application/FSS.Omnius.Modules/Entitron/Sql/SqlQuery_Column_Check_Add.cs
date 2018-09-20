using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Column_Check_Add : SqlQuery_withAppTable
    {
        public string where { get; set; }
        public string realCheckName { get; set; }
        public string checkName
        {
            get
            {
                return string.Join("_", realCheckName.Split('_').Skip(3));
            }
            set
            {
                realCheckName = $"CHK_{application.Name}_{table.Name}_{value}";
            }
        }

        protected override string CreateString()
        {
            if (string.IsNullOrWhiteSpace(realCheckName))
                checkName = "1";

            return
                $"ALTER TABLE [{realTableName}] ADD CONSTRAINT {realCheckName} CHECK {where};";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{table.Name}] Add check[{checkName}] '{where}'";
        }
    }
}
