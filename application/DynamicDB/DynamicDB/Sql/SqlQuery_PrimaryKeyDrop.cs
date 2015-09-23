﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_PrimaryKeyDrop : SqlQuery_withApp
    {
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTableName = safeAddParam("tableName", tableName);

            _sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName@{0},@{1}, @realTableName OUTPUT;" +
                "SET @sql=CONCAT('ALTER TABLE ', @realTableName, 'DROP CONSTRAINT PK_', @realTableName, ';')"+ 
                "exec(@sql)",
                parAppName, parTableName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Drop primary key in {0}[{1}]", tableName, applicationName);
        }
    }

}
