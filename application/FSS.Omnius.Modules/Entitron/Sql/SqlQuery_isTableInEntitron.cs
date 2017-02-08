using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_isTableInEntitron : SqlQuery_withApp
    {
        public string tableName { get; set; }

        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            string realName = $"Entitron_{(this.application.Id == SharedTables.AppId ? SharedTables.Prefix : this.application.Name)}_{tableName}";


            sqlString = $"SELECT Distinct TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{realName}'";

            return base.BaseExecutionWithRead(connection);
        }
    }
}
