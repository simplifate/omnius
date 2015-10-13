using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Entitron.Sql
{
    class SqlQuery_Table_Rename : SqlQuery_withApp
    {
        public string newName{ get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parNewName = safeAddParam("newName", newName);

            sqlString =string.Format(
                "DECLARE @MetaTables NVARCHAR(100) = (SELECT DbMetaTables FROM {3} WHERE Name = @{0});" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('UPDATE ', @MetaTables, ' SET Name = @{1} WHERE Name = @{2};');" +
                "exec sp_executesql @sql, N'@{2} NVARCHAR(50), @{1} NVARCHAR(50)', @{2} = @{2}, @{1} = @{1};",
                parAppName,parNewName,parTableName, DB_MasterApplication);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Table rename {0} in [{1}]", table.tableName, application.Name);
        }
    }
}
