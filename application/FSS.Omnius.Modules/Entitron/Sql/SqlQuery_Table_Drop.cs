﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_Table_Drop : SqlQuery_withApp
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);

            sqlString =string.Format(
                "DECLARE @_tempTable table(tableId INT);DECLARE @_tableId INT,@_sql NVARCHAR(MAX);" +
                "DELETE e OUTPUT DELETED.tableId INTO @_tempTable FROM {1} e INNER JOIN {0} a ON a.Id = e.ApplicationId WHERE e.Name = @{3} AND a.Name = @{2};" +
                "SET @_tableId = (SELECT tableId FROM @_tempTable);SET @_sql = CONCAT('DROP TABLE ', object_name(@_tableId));" +
                "exec (@_sql);",
                DB_MasterApplication,
                DB_EntitronMeta,
                parAppName,
                parTableName
                );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop table {0} in [{1}]", table.tableName, application.Name);
        }
    }
}
