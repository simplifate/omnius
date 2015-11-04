using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    class SqlQuery_ForeignKeyAdd : SqlQuery_withApp
    {
        public DBTable table2 { get; set; }
        public string foreignKey { get; set; }
        public string primaryKey { get; set; }
        public string foreignName { get; set; }
        public string onDelete { get; set; }
        public string onUpdate { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string update = (onUpdate == "cascade")
                ? " ON UPDATE CASCADE"
                : (onUpdate == "null")
                    ? " ON UPDATE SET NULL"
                    : (onUpdate == "default") ? " ON UPDATE SET DEFAULT" : ""; ;
            string delete = (onDelete == "cascade")
                ? " ON DELETE CASCADE"
                : (onDelete == "null")
                    ? " ON DELETE SET NULL"
                    : (onDelete == "default") ?" ON DELETE SET DEFAULT" : "";

            string parAppName = safeAddParam("applicationName", application.Name);
            string parTable1Name = safeAddParam("tableName", table.tableName);
            string parTable2Name = safeAddParam("tableName", table2.tableName);
            string parForeignKey = safeAddParam("foreignKey", foreignKey);
            string parPrimaryKey = safeAddParam("primaryKey", primaryKey);
            string parForeignName = safeAddParam("foreignName", foreignName);

            sqlString = string.Format(
                "DECLARE @realTable1Name NVARCHAR(50), @realTable2Name NVARCHAR(50), @sql NVARCHAR(MAX);" +
                "exec getTableRealName @{0}, @{1}, @realTable1Name OUTPUT;" +
                "exec getTableRealName @{0}, @{2}, @realTable2Name OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTable1Name, ' ADD CONSTRAINT FK_{8}_', @{3},' FOREIGN KEY (', @{4}, ') REFERENCES ', @realTable2Name, ' (', @{5}, ') " +
                " {6} {7} ;');" +
                "exec (@sql);",
                parAppName, parTable1Name, parTable2Name, parForeignName, parForeignKey, parPrimaryKey,delete,update,application.Name);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add Foreign key to {0} in {1}[{2}]", table2.tableName, table.tableName, application.Name);
        }
    }
}
