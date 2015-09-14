﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Modify:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public DBColumn column{ get; set; }

        public SqlQuery_Column_Modify(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", _applicationName);
            string parTableName = safeAddParam("tableName", tableName);
            var parColumn = safeAddParam("columnDefinition", column.getSqlDefinition());

            _sqlString =string.Format(
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ALTER COLUMN ', @{2});" +
                "exec(@sql);", parAppName, parTableName, parColumn);

            base.BaseExecution(transaction);
        }
    }
}
