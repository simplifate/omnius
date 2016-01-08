using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
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
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += $"{_conditions._concat} ([{_conditions._columnName}]=@{parValue})";

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotEqual(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}]<>@{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Greater(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}]>@{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat GreaterOrEqual(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}]>=@{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Less(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat= _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}]<@{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat LessOrEqual(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}]<=@{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Between(object value, object value2)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            string parValue2 = _conditions._query.safeAddParam("value", value2);
            _conditions._sql += string.Format("{0}([{1}] BETWEEN @{2} AND @{3})", _conditions._concat, _conditions._columnName, parValue, parValue2);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotBetween(object value, object value2)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            string parValue2 = _conditions._query.safeAddParam("value", value2);
            _conditions._sql += string.Format("{0}([{1}] NOT BETWEEN @{2} AND @{3})", _conditions._concat, _conditions._columnName, parValue, parValue2);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Like(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}] LIKE @{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotLike(object value)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");

            string parValue = _conditions._query.safeAddParam("value", value);
            _conditions._sql += string.Format("{0}([{1}] NOT LIKE @{2})", _conditions._concat, _conditions._columnName, parValue);

            return new Condition_concat(_conditions);
        }
        public Condition_concat Null()
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");
            
            _conditions._sql += string.Format("{0}([{1}] IS NULL)", _conditions._concat, _conditions._columnName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotNull()
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");
            
            _conditions._sql += string.Format("{0}([{1}] IS NOT NULL)", _conditions._concat, _conditions._columnName);

            return new Condition_concat(_conditions);
        }
        public Condition_concat In(List<object> values)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");
            
            _conditions._sql += string.Format(" [{0}] IN ({1})", _conditions._columnName, string.Join(", ", values));

            return new Condition_concat(_conditions);
        }
        public Condition_concat NotIn(List<object> values)
        {
            if (_conditions.isCheck)
                _conditions._concat = _conditions._concat.Replace("WHERE", "");
            
            _conditions._sql += string.Format(" [{0}] NOT IN ({1})", _conditions._columnName, string.Join(", ", values));

            return new Condition_concat(_conditions);
        }
    }
}
