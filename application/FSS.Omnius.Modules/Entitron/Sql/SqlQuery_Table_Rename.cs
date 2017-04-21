using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_Rename : SqlQuery_withAppTable
    {
        public string newName{ get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string newRealTableName = $"Entitron_{(application.Id == SharedTables.AppId ? SharedTables.Prefix : application.Name)}_{newName}";

            string parAppName = safeAddParam("ApplicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parNewName = safeAddParam("newName", newName);

            sqlString =
                $"UPDATE {DB_EntitronMeta} SET Name = @{parNewName} WHERE Name = @{parTableName} AND ApplicationName = @{parAppName};" +
                $"sp_rename '{realTableName}', '{newRealTableName}';";

                //string.Format(
                //"DECLARE @sql NVARCHAR(MAX);" +
                //"SET @sql= CONCAT('UPDATE {2} SET Name = ', '''', @{0}, '''' ,' WHERE Name = ', '''', @{1}, '''',' ;');" +
                //"exec sp_executesql @sql, N'@{0} NVARCHAR(50), @{1} NVARCHAR(50)', @{0}, @{1};",
                //parNewName,parTableName, DB_EntitronMeta);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Table rename {0} to {2} in [{1}]", table.tableName, application.Name, newName);
        }
    }
}
