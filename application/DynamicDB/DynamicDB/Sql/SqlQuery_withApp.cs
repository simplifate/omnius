using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class SqlQuery_withApp : SqlQuery
    {
        protected string _applicationName;

        public SqlQuery_withApp(string applicationName, string SqlString = "", Dictionary<string, object> param = null) : base(SqlString, param)
        {
            _applicationName = applicationName;
        }
    }
}
