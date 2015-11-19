using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Entitron.Sql
{
    class SqlQuery_PrimaryKeyAdd : SqlQuery_withApp
    {
        public List<string> keyColumns { get; set; }
        public bool isClusterCreated { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (keyColumns == null || keyColumns.Count < 1)
                throw new ArgumentNullException("keyColumn");

            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parColumns = safeAddParam("columns", string.Join(",", keyColumns));

            if (isClusterCreated != true)
            {
                sqlString = string.Format(
                    "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                    "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_{3}_{4}_', @realTableName, ' PRIMARY KEY NONCLUSTERED (', @{2}, ');' , " +
                    "' CREATE CLUSTERED INDEX index_', @{0}, @{1} , ' ON ', @realTableName, '(', @{2},');');" +
                    "exec (@sql)",
                    parAppName, parTableName, parColumns, application.Name, table.tableName);
            }
            else
            {
                sqlString = string.Format(
                    "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                    "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_{3}_{4}_', @realTableName, ' PRIMARY KEY NONCLUSTERED (', @{2}, ');' );" +
                    "exec (@sql)",
                    parAppName, parTableName, parColumns, application.Name, table.tableName);
            }


            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add primary key to {0}[{1}]", table.tableName, application.Name);
        }
    }
}
