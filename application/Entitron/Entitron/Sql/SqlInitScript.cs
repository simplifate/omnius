using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlInitScript : SqlQuery
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString = string.Format(
                "CREATE PROCEDURE getTableRealName @applicationName NVARCHAR(50), @tableName NVARCHAR(50), @realTableName NVARCHAR(50) OUTPUT AS " +
                "SET @realTableName = (SELECT object_name( (SELECT tableId FROM {1} e INNER JOIN {0} a ON e.ApplicationId = a.Id WHERE e.Name = @tableName AND a.Name = @applicationName) ))",
                DB_MasterApplication,
                DB_EntitronMeta
                );
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return "Initial query: creating procedure";
        }
    }
}
