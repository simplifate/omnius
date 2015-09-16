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
            string parAppName = safeAddParam("AppName", _applicationName);
            string parTableName = safeAddParam("tableName", _tableName);
            string parColumnDef = safeAddParam("columnDefinition", string.Join(",", _columns.Select(c => c.getSqlDefinition())));

            _sqlString = string.Format(
                "DECLARE @_DbMetaTables NVARCHAR(50), @_sql NVARCHAR(MAX);" +
                "SELECT @_DbMetaTables = DbMetaTables FROM {0} WHERE Name = @{1};" +
                "SET @_sql = CONCAT('INSERT INTO ', @_DbMetaTables, '(Name) VALUES(@{2});');" +
                "exec sp_executesql @_sql, N'@{2} NVARCHAR(50)', @{2};" +
                "DECLARE @realTableName NVARCHAR(50),@sql NVARCHAR(MAX);exec getTableRealName @{1}, @{2}, @realTableName OUTPUT;" +
                "SET @sql = CONCAT('CREATE TABLE ', @realTableName, '(', @{3}, ');');" +
                "exec(@sql);",
                SqlInitScript.aplicationTableName,
                parAppName,
                parTableName,
                parColumnDef
                );
            
            base.BaseExecution(transaction);
        }
    }
}
