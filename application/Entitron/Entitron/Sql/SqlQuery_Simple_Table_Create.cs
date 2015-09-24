using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Entitron.Sql
{
    public class SqlQuery_Simple_Table_Create : SqlQuery
    {
        private string _tableName;
        public string tableName { get { return _tableName; } }
        private List<DBColumn> _columns = new List<DBColumn>();

        public SqlQuery_Simple_Table_Create(string tableName) : base()
        {
            _tableName = tableName;
        }

        public SqlQuery_Simple_Table_Create AddColumn(DBColumn column)
        {
            _columns.Add(column);

            return this;
        }
        public SqlQuery_Simple_Table_Create AddColumn(string columnName, SqlDbType type, int? maxLength = null, bool canBeNull = true, string additionalOptions = null)
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

        public SqlQuery_Simple_Table_Create AddParameters(string parameters)
        {
            _columns.Add(new AdditionalSqlDefinition() { definition = parameters });

            return this;
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            _sqlString = string.Format(
                "CREATE TABLE {0}({1});",
                tableName,
                string.Join(",", _columns.Select(c => c.getSqlDefinition()))
                );

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Create table {0}", tableName);
        }
    }
}
