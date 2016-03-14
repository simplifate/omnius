﻿using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_SelectSpecificDefault:SqlQuery_withApp
    {
        public string columnName { get; set; }
        protected override ListJson<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parColumnName = safeAddParam("column", columnName);

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50); exec getTableRealName @{0}, @{1}, @realTableName output;" +
                "SELECT d.name name, d.definition def FROM sys.default_constraints d " +
                "INNER JOIN sys.tables t ON t.object_id=d.parent_object_id " +
                "WHERE t.name= @realTableName and d.name like '%' + @{2}",
                parAppName, parTableName, parColumnName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
