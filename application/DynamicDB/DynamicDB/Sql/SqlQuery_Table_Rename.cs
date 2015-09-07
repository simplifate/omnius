using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DynamicDB.Sql
{
    class SqlQuery_Table_Rename : SqlQuery_withApp
    {
        private string _tableName;
        public string tableName { get { return _tableName; } }
        public string newName;

        public SqlQuery_Table_Rename(string ApplicationName, string TableName, string newName = null) : base(ApplicationName)
        {
            _tableName = TableName;
            this.newName = newName;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _params.Add("applicationName", _applicationName);
            _params.Add("tableName", _tableName);
            _params.Add("newName", this.newName);

            _sqlString =
                "DECLARE @MetaTables NVARCHAR(100) = (SELECT DbMetaTables FROM dbo.Applications WHERE Name = @applicationName);" +
                "DECLARE @sql NVARCHAR(MAX) = CONCAT('UPDATE ', @MetaTables, ' SET Name = @newName WHERE Name = @tableName;');" +
                "exec sp_executesql @sql, N'@tableName NVARCHAR(50) output, @newName NVARCHAR(50)', @tableName = @tableName, @newName = @newName;";

            base.BaseExecution(transaction);
        }
    }
}
