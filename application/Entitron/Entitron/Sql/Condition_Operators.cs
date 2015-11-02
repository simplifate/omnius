using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Sql
{
    public class Condition_Operators
    {
        private Conditions _conditions;

        public Condition_Operators(Conditions conditions)
        {
            _conditions = conditions;
        }

        public Condition_concat Equal(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }
            _conditions._sql += string.Format("{0}({1}=@{2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotEqual(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }
            _conditions._sql += string.Format("{0}({1}<>{2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Greater(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }
            _conditions._sql += string.Format("{0}({1}>{2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat GreaterOrEqual(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1}>={2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Less(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1}<{2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat LessOrEqual(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1}<={2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Between(object value, object value2)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            string paramName2 = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} BETWEEN {2} AND {3})", _conditions._concat, _conditions._columnName, paramName, paramName2);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotBetween(object value, object value2)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            string paramName2 = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} NOT BETWEEN {2} AND {3})", _conditions._concat, _conditions._columnName, paramName, paramName2);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Like(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} LIKE {2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotLike(object value)
        {
            string paramName = _conditions._query.safeAddParam("param", value);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} NOT LIKE {2})", _conditions._concat, _conditions._columnName, paramName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Null()
        {
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} IS NULL)", _conditions._concat, _conditions._columnName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotNull()
        {
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format("{0}({1} IS NOT NULL)", _conditions._concat, _conditions._columnName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat In(List<object> values)
        {
            string paramNames = _conditions._query.safeAddParam("param", values);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format(" {0} IN ({1})", _conditions._columnName, string.Join(", ", paramNames) );

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotIn(List<object> values)
        {
            string paramNames = _conditions._query.safeAddParam("param", values);
            if (_conditions.isCheck)
            {
                _conditions._concat = "";
            }

            _conditions._sql += string.Format(" {0} NOT IN ({1})", _conditions._columnName, string.Join(", ", paramNames));

            return new Condition_concat(_conditions);
        }
    }
}
