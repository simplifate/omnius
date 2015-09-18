using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_ForeignKeyAdd:SqlQuery_withApp
    {
        public string table2Name { get; set; }
        public List<string> foreignKey { get; set; }
        public string primaryKey { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", applicationName);
            string parTable1Name = safeAddParam("tableName", tableName);
            string parTable2Name = safeAddParam("tableName", table2Name);
            string parForeignKey = safeAddParam("foreignKey", string.Join(",", foreignKey));
            string parPrimaryKey = safeAddParam("primaryKey", string.Join(",", primaryKey));

            _sqlString = string.Format(
                "DECLARE @realTable1Name NVARCHAR(50), @realTable2Name NVARCHAR(50), @sql NVARCHAR(MAX);" +
                "exec getRealTableName@{0},@{1}, @realTable1Name OUTPUT;" +
                "exec getRealTableName@{0},@{2}, @realTable2Name OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTable1Name, ' ADD CONSTRAINT FK_', @realTable1Name,' FOREIGN KEY (', {3}, ') REFERENCES ', @realTable2Name, '(', {4}, ');')",
                parAppName,parTable1Name,parTable2Name,parForeignKey,parPrimaryKey);

            base.BaseExecution(transaction);
        }
    }
}
