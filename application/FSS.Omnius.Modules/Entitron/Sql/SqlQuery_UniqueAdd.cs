using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_UniqueAdd : SqlQuery_withAppTable
    {
        public string keyColumns { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string realTableName = $"Entitron_{application.Name}_{table.tableName}";

            //string parAppName = safeAddParam("applicationName", application.Name);
            //string parTableName = safeAddParam("tableName", table.tableName);
            //string parColumns = safeAddParam("columns", keyColumns);

            sqlString =
                $"ALTER TABLE {realTableName} ADD CONSTRAINT UN_{realTableName}_{keyColumns} UNIQUE({keyColumns});";

                //string.Format(
                //"DECLARE @realTableName NVARCHAR(100) @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                //"SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT UN_', @realTableName, '_', @{2}, ' UNIQUE (', @{2}, ');')" +
                //"exec (@sql)",
                //parAppName, parTableName, parColumns);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add unique in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
