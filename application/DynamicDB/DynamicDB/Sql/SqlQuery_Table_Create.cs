using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DynamicDB.Sql
{
    class SqlQuery_Table_Create : SqlQuery_withApp
    {
        private string _tableName;
        public string tableName { get { return _tableName; } }
        private List<DBColumn> _columns = new List<DBColumn>();

        public SqlQuery_Table_Create(string applicationName, string tableName) : base(applicationName)
        {
            _tableName = tableName;
        }

        public SqlQuery_Table_Create AddColumn(DBColumn column)
        {
            _columns.Add(column);

            return this;
        }
        public SqlQuery_Table_Create AddColumn(string columnName, SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
        {
            _columns.Add(new DBColumn()
            {
                Name = columnName,
                type = type,
                maxLength = maxLength,
                canBeNull = canBeNull,
                additionalOptions = additionalOptions
            });

            return this;
        }

        public SqlQuery_Table_Create AddParameters(string parameters)
        {
            _columns.Add(new AdditionalSqlDefinition() { definition = parameters });

            return this;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("tableName", _tableName);
            _params.Add("applicationName", _applicationName);
            _params.Add("columnDefinition", string.Join(",", _columns.Select(c => c.getSqlDefinition())));
            _sqlString =
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @applicationName, @tableName, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('CREATE TABLE ', @realTableName, '(', @columnDefinition, ');');" +
                "exec(@sql);";
            
            base.BaseExecution(transaction);
        }
    }
}
