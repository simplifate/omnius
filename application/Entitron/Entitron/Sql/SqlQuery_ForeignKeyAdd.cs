using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_ForeignKeyAdd : SqlQuery_withApp
    {
        public string table2Name { get; set; }
        public string foreignKey { get; set; }
        public string primaryKey { get; set; }
        public string foreignName { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTable1Name = safeAddParam("tableName", tableName);
            string parTable2Name = safeAddParam("tableName", table2Name);
            string parForeignKey = safeAddParam("foreignKey", foreignKey);
            string parPrimaryKey = safeAddParam("primaryKey", primaryKey);
            string parForeignName = safeAddParam("foreignName", foreignName);

            _sqlString = string.Format(
                "DECLARE @realTable1Name NVARCHAR(50), @realTable2Name NVARCHAR(50), @sql NVARCHAR(MAX);" +
                "exec getTableRealName @{0}, @{1}, @realTable1Name OUTPUT;" +
                "exec getTableRealName @{0}, @{2}, @realTable2Name OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTable1Name, ' ADD CONSTRAINT FK_', @{3},' FOREIGN KEY (', @{4}, ') REFERENCES ', @realTable2Name, ' (', @{5}, ');');" +
                "exec (@sql);",
                parAppName, parTable1Name, parTable2Name, parForeignName, parForeignKey, parPrimaryKey);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add Foreign key to {0} in {1}[{2}]", table2Name, tableName, applicationName);
        }
    }
}
