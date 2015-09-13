using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_Column_Modify:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public DBColumn column{ get; set; }

        public SqlQuery_Column_Modify(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", tableName);
            _params.Add("columnDefinition", column.getSqlDefinition());

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('ALTER TABLE ', @realTableName, ' ALTER COLUMN ', @columnDefinition);" +
                "exec(@sql);";

            base.BaseExecution(transaction);
        }
    }
}
