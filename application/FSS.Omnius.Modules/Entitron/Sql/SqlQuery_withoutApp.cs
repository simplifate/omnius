using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_withoutApp : SqlQuery
    {
        public override string connectionString { get; }

        public SqlQuery_withoutApp(string connection) : base()
        {
            connectionString = connection;
        }
    }
}
