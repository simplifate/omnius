using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class SqlQuery_Selectable : SqlQuery_withApp
    {
        internal string _join = "";
        internal string _where = "";
        internal string _order = "";
        internal string _group = "";

        public string tableName { get; set; }

        public SqlQuery_Selectable(string ApplicationName) : base(ApplicationName)
        {
        }

        public Operators where(string columnName)
        {
            return new Operators(this, columnName);
        }
        public SqlQuery_Selectable join(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join += string.Format(" JOIN {0} ON {1}.{2}={3}{4}", joinedTableName, tableName, originColumnName, joinedTableName, joinedColumnName);

            return this;
        }
        public SqlQuery_Selectable order(string columnName)
        {
            _order = string.Format(" ORDER BY {0}", columnName);

            return this;
        }
        public SqlQuery_Selectable group(string columnName)
        {
            _group = string.Format(" GROUP BY {0}", columnName);

            return this;
        }
    }
}
