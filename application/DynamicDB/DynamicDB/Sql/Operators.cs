using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDB.Sql
{
    public class Operators
    {
        private SqlQuery_Selectable _query;
        private string _columnName;

        public Operators(SqlQuery_Selectable query, string columnName)
        {
            _query = query;
            _columnName = columnName;
        }

        public SqlQuery_Selectable Equal(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}={1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable NotEqual(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}<>{1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable Greater(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}>{1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable GreaterOrEqual(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}>={1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable Less(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}<{1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable LessOrEqual(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0}<={1}", _columnName, paramName);

            return _query;
        }
        public SqlQuery_Selectable Between(object value, object value2)
        {
            string paramName1 = _query.safeAddParam("param", value);
            string paramName2 = _query.safeAddParam("param2", value2);
            _query._where += string.Format(" {0} BETWEEN {1} AND {2}", _columnName, paramName1, paramName2);

            return _query;
        }
        public SqlQuery_Selectable Like(object value)
        {
            string paramName = _query.safeAddParam("param", value);
            _query._where += string.Format(" {0} LIKE {1}", _columnName, paramName);

            return _query;
        }
        //public SqlQuery_Selectable In(object value)
        //{
        //    string paramName = _query.safeAddParam("param", value);
        //    _query._where += string.Format(" {0}<>{1}", _columnName, paramName);

        //    return _query;
        //}
    }
}
