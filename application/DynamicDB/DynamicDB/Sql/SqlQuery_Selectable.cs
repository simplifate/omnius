using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class SqlQuery_Selectable : SqlQuery_withApp
    {
        internal List<string> _join = new List<string>();
        internal Condition_concat _where = null;
        internal string _order = "";
        internal string _group = "";
        
        public SqlQuery_Selectable where(Func<Conditions, Condition_concat> conditions)
        {
            _where = conditions(new Conditions(this));

            return this;
        }
        public SqlQuery_Selectable join(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, tableName, originColumnName, joinedColumnName));

            return this;
        }
        public SqlQuery_Selectable leftOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" LEFT OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, tableName, originColumnName, joinedColumnName));

            return this;
        }
        public SqlQuery_Selectable rightOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" RIGHT OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, tableName, originColumnName, joinedColumnName));

            return this;
        }
        public SqlQuery_Selectable fullOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" FULL OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, tableName, originColumnName, joinedColumnName));

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
