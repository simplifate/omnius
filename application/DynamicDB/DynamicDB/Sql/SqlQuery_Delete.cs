using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicDB.Table;

namespace DynamicDB.Sql
{
    class SqlQuery_Delete:SqlQuery_withApp
    {
        public string tableName { get; set; }
        public DBColumn keyCol { get; set; }
        public DBValue value { get; set; }

        public SqlQuery_Delete(string applicationName) : base(applicationName)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("tableName", tableName);
            _params.Add("applicationName", _applicationName);
            _params.Add("keyColumn", keyCol.Name);
            _params.Add("keyValue", value);

            _sqlString = "DECLARE @realTableName NVARCHAR(50), @sql NVARCHAR(MAX); exec getRealTableName @applicationName, @tableName, @realTableName OUTPUT;"+
                "SET @sql= CONCAT('DELETE FROM ', @realTableName, ' WHERE ', @keyColumn, '=', @keyValue, ';')"+
                "exec (@sql)";
           


            base.BaseExecution(transaction);
        }
    }
}
