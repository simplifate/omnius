using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class SqlQuery_withApp : SqlQuery
    {
        public string applicationName;
        public string tableName;

        public SqlQuery_withApp(string applicationName = null, string SqlString = "", Dictionary<string, object> param = null) : base(SqlString, param)
        {
            this.applicationName = applicationName;
        }

        protected override void BaseExecution(MarshalByRefObject connection)
        {
            if (applicationName == null)
                throw new ArgumentNullException("applicationName");
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            base.BaseExecution(connection);
        }
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (applicationName == null)
                throw new ArgumentNullException("applicationName");
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            return base.BaseExecutionWithRead(connection);
        }
    }
}
