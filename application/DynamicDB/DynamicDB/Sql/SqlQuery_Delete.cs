﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Delete:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public Dictionary<DBColumn, object> columnValueCondition { get; set; }

        public SqlQuery_Delete(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("AppName", _applicationName);
            string parTableName = safeAddParam("TableName", tableName);
            var parConditions = safeAddParam(columnValueCondition);

            _sqlString = "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @applicationName, @tableName, @realTableName OUTPUT;"+
                "SET @sql= CONCAT('DELETE FROM ', @realTableName, ' WHERE " + string.Join(" AND ", parConditions.Select(pair => pair.Key.Name + "=@" + pair.Key.Name)) + ";')"+
                "exec sp_executesql @sql, N'" + string.Join(",", parConditions.Select(pair => pair.Key.getShortSqlDefinition() )) + "', " + string.Join(",", parConditions.Select(pair => string.Format("@{0}=@{1}", pair.Key.Name, pair.Value)));

            base.BaseExecution(transaction);
        }
    }
}
