using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Tabloid_Exists : SqlQuery_withApp
    {
        public string tabloidName;

        protected override string CreateString()
        {
            if (string.IsNullOrEmpty(tabloidName))
                throw new ArgumentNullException("tabloidName");

            string parTabloidName = safeAddParam("realName", RealTableName(application, tabloidName));

            return
                $"SELECT name FROM sys.tables WHERE name = @{parTabloidName} UNION SELECT name FROM sys.views WHERE name = @{parTabloidName}";
        }

        public override string ToString()
        {
            return $"[{application.Name}:{tabloidName}] Exists table or view?";
        }
    }
}
