using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_PrimaryKeyAdd : SqlQuery_withAppTable
    {
        public string keyColumns { get; set; }
        public bool isClusterCreated { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {

            string parAppName = safeAddParam("AppName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parColumns = safeAddParam("columns", keyColumns);

            if (isClusterCreated != true)
            {
                sqlString = string.Format(
                    "DECLARE @realTableName NVARCHAR(100), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                    "SET @sql= CONCAT('ALTER TABLE ',@realTableName, ' ALTER COLUMN ', @{2}, ' INT IDENTITY;',' ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_', @realTableName, ' PRIMARY KEY NONCLUSTERED (', @{2}, ');' , " +
                    "' CREATE CLUSTERED INDEX index_', @{0}, @{1} , ' ON ', @realTableName, '(', @{2},');');" +
                    "exec (@sql)",
                    parAppName, parTableName, parColumns);
            }
            else
            {
                sqlString = string.Format(
                    "DECLARE @realTableName NVARCHAR(100), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                    "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT PK_', @realTableName, ' PRIMARY KEY NONCLUSTERED (', @{2}, ');' );" +
                    "exec (@sql)",
                    parAppName, parTableName, parColumns);
            }


            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add primary key to {0}[{1}]", table.tableName, application.Name);
        }
    }
}
