using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_Rename : SqlQuery_withApp
    {
        public string newName{ get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parTableName = safeAddParam("tableName", table.tableName);
            string parNewName = safeAddParam("newName", newName);

            sqlString =string.Format(
                "DECLARE @sql NVARCHAR(MAX);" +
                "SET @sql= CONCAT('UPDATE {2} SET Name = ', '''', @{0}, '''' ,' WHERE Name = ', '''', @{1}, '''',' ;');" +
                "exec sp_executesql @sql, N'@{0} NVARCHAR(50), @{1} NVARCHAR(50)', @{0}, @{1};",
                parNewName,parTableName, DB_EntitronMeta);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Table rename {0} in [{1}]", table.tableName, application.Name);
        }
    }
}
