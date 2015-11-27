using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_UniqueAdd : SqlQuery_withApp
    {
        public List<string> keyColumns { get; set; }
        public string uniqueName { get; set; }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTableName = safeAddParam("tableName", table.tableName);
            string parUniqueName = safeAddParam("uniqueName", uniqueName);
            string parColumns = safeAddParam("columns", string.Join(",", keyColumns));

            sqlString = string.Format(
                "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT UN_{4}_{5}_', @{2}, ' UNIQUE (', @{3}, ');')" +
                "exec (@sql)",
                parAppName, parTableName, parUniqueName ,parColumns,application.Name, table.tableName);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add unique in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
