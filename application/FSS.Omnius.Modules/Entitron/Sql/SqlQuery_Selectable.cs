using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    public class SqlQuery_Selectable<T> : SqlQuery_withAppTable where T : SqlQuery_Selectable<T>
    {
        internal List<string> _join = new List<string>();
        internal Condition_concat _where = null;
        internal string _order = "";
        internal string _group = "";
        
        public T where(Func<Conditions, Condition_concat> conditions)
        {
            _where = (_where != null)
                ? _where = _where.and(conditions(new Conditions(this)))
                : _where = conditions(new Conditions(this));

            return (T)this;
        }
        public T join(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, table.tableName, originColumnName, joinedColumnName));

            return (T)this;
        }
        public T leftOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" LEFT OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, table.tableName, originColumnName, joinedColumnName));

            return (T)this;
        }
        public T rightOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" RIGHT OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, table.tableName, originColumnName, joinedColumnName));

            return (T)this;
        }
        public T fullOuterJoin(string joinedTableName, string originColumnName, string joinedColumnName)
        {
            _join.Add(string.Format(" FULL OUTER JOIN {0} ON {1}.{2}={0}{3}", joinedTableName, table.tableName, originColumnName, joinedColumnName));

            return (T)this;
        }
 
        public T order(string columnName)
        {
            if (!string.IsNullOrWhiteSpace(columnName))
                _order = string.Format(" ORDER BY {0}", columnName);

            return (T)this;
        }
        public T orderDesc(string columnName)
        {
            if (!string.IsNullOrWhiteSpace(columnName))
                _order = string.Format(" ORDER BY {0} DESC", columnName);

            return (T)this;
        }
        public T group(string columnName)
        {
            _group = string.Format(" GROUP BY {0}", columnName);

            return (T)this;
        }
    }
}
