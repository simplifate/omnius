using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicDB.Table;

namespace DynamicDB.Sql
{
    class SqlQuery_Update:SqlQuery_withApp
    {
        public string tableName;
        public List<DBColumn> columns { get; set; }
        public List<DBValue> values { get; set; }
        public DBColumn keyCol;
        public object keyVal;


        public SqlQuery_Update(string applicationName, string tableName, DBColumn keyCol, string keyVal) : base(applicationName)
        {
            this.tableName = tableName;
            this.keyCol = keyCol;
            this.keyVal = keyVal;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("tableName", tableName);
            _params.Add("applicationName", _applicationName);
            _params.Add("keyColumn", keyCol);
            _params.Add("keyValue", keyVal);

            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql= CONCAT('UPDATE ', @realTableName, ' SET '";
            for(int i=0;i<=columns.Count;i++)
            {
               
                string columName = safeAddParam("columnName", columns[i]);
                string valueName = safeAddParam("value", values[i]);
                
                values[i].setDataType(valueName,columns[i].type,columns[i].maxLength);
                
                _sqlString += string.Format(", @{0}, '=', @{1}", columName,valueName);
            }
            _sqlString+="' WHERE ', @keyColumn, '=', @keyValue, ';')"+
                    "exec sp_executesql @sql, N'" + string.Join(",", values.Select(val=>val.getDataType())) + "', " + string.Join(",", values.Select(vn => vn + "=" + vn)) + ";";    
           

            base.BaseExecution(transaction);
        }
    }
}
