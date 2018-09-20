using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlInitScript_procedure : SqlQuery_withoutApp
    {
        public SqlInitScript_procedure(string connection) : base(connection)
        {
        }

        protected override string CreateString()
        {
            return "";
        }

        public override string ToString()
        {
            return "Initial query: creating procedures";
        }
    }
}
