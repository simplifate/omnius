using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicDB.Table;

namespace DynamicDB.Sql
{
    class SqlQuery_Insert : SqlQuery_withApp
    {
        public List<DBColumn> columns { get; set; }
        public string tableName { get; set; }
        public List<DBValue> values { get; set; }

        public SqlQuery_Insert(string applicationName) : base(applicationName)
        {
        }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentNullException("values");

            _params.Add("tableName", tableName);
            _params.Add("applicationName", _applicationName);
            if (columns != null && columns.Count > 0)
                _params.Add("columnsNames", string.Join(",", columns.Select(dbc => dbc.Name)));
            
            List<string> valueNames = new List<string>();
            
            for(int i=0;i<=values.Count;i++)
            {
                valueNames.Add("@" + safeAddParam("value", values[i]));

                 values[i].setDataType(valueNames[i], columns[i].type,columns[i].maxLength);
            }

            if (columns!=null)
            {
                _sqlString =
                    "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                    "SET @sql= CONCAT('INSERT INTO ', @realTableName, ' (', @columnNames, ') VALUES (" + string.Join(",", valueNames) + ");'" + 
                    "exec sp_executesql @sql, N'"+ string.Join(",",values.Select(val=>val.getDataType()) ) +"', "+ string.Join(",", valueNames) +";";    
            }
            // TODO how to make a statement without a specified columns

            base.BaseExecution(transaction);
        }
   

    }
}
