using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    class SqlQuery_PrimaryKeyAdd:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public List<DBColumn> keyColumns { get; set; }
        public string constraintName { get; set; } 

        public SqlQuery_PrimaryKeyAdd(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("tableName", tableName);
            _params.Add("applicationName", _applicationName);
            _params.Add("columns",(keyColumns!=null && keyColumns.Count>0)?string.Join(",",keyColumns):"");
            _params.Add("constraintName", constraintName);

            _sqlString = "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('ALTER TABLE ', @realTableName, ' ADD CONSTRAINT', @constraintName, ' PRIMARY KEY (', @columns, ');')" +
                "exec (@sql)";



            base.BaseExecution(transaction);
        }
    }
}
