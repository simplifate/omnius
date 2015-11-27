using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_InsertSelect : SqlQuery_withApp
    {
        public string table2Name { get; set; }
        public List<string> columns1 { get; set; }
        public List<string> columns2 { get; set; }
        public string where { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string parAppName = safeAddParam("applicationName", application.Name);
            string parTable1Name = safeAddParam("tableName", table.tableName);
            string parTable2Name = safeAddParam("tableName", table2Name);
            string parColumn1Name = safeAddParam("columnName", string.Join(", ", columns1));
            string parColumn2Name = safeAddParam("columnName", string.Join(", ", columns2));
            string parWhere = safeAddParam("where", where);

            sqlString = string.Format(
                "DECLARE @realTable1Name NVARCHAR(50), @realTable2Name NVARCHAR(50), @sql NVARCHAR(MAX); exec getTableRealName @{0}, @{1}, @realTable1Name OUTPUT; exec getTableRealName @{0}, @{2}, @realTable2Name OUTPUT;" +
                "SET @sql=CONCAT('INSERT INTO ', @realTable1Name, '(', @{3},') SELECT ', @{4},' FROM ', @realTable2Name,' ', @{5}, ';');"+
                "exec (@sql);",
                parAppName,parTable1Name,parTable2Name, parColumn1Name, parColumn2Name, parWhere);

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Insert row in {0}[{1}]", table.tableName, application.Name);
        }
    }
}
