using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlInitScript : SqlQuery_withoutApp
    {
        public SqlInitScript(string connection) : base(connection)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString = 
                "CREATE PROCEDURE getTableRealName @applicationName NVARCHAR(50), @tableName NVARCHAR(50), @realTableName NVARCHAR(100) OUTPUT AS " +
                "SET @realTableName = CONCAT('Entitron_', @applicationName, '_', @tableName);";

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return "Initial query: creating procedure";
        }
    }
}
