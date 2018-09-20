using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_List : SqlQuery_withApp
    {
        protected override string CreateString()
        {
            string parAppName = safeAddParam("applicationName", $"Entitron_{application.Name}_%");
            string parLength = safeAddParam("length", application.Name.Length + 11);

            return
                $"SELECT SUBSTRING([Name], @{parLength}, LEN([Name])) [Name] FROM sys.tables WHERE [Name] LIKE @{parAppName};";
        }

        public override string ToString()
        {
            return $"[{application.Name}] List tables";
        }
    }
}
