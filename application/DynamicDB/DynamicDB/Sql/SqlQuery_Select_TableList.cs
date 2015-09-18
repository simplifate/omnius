using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Select_TableList : SqlQuery
    {
        public string ApplicationName;
        public List<string> columns { get; set; }
        
        protected override List<DBItem> BaseExecutionWithRead(MarshalByRefObject connection)
        {
            if (ApplicationName == null)
                throw new ArgumentNullException("ApplicationName");

            string parAppName = safeAddParam("applicationName", ApplicationName);
            string parColumnName = safeAddParam("columnNames",
                (columns != null && columns.Count > 0) ? string.Join(",", columns) : "*");

            _sqlString =string.Format(
                "DECLARE @tableName NVARCHAR(50) = (SELECT DbMetaTables FROM {0} WHERE Name = @{1});" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('SELECT ', @{2}, ' FROM ', @tableName, ';');" +
                "exec(@sql);", SqlInitScript.aplicationTableName, parAppName,parColumnName);

            return base.BaseExecutionWithRead(connection);
        }
    }
}
