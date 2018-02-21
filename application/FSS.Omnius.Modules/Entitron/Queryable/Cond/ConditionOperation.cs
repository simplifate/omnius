using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Queryable.Cond
{
    public class ConditionOperation
    {
        internal ConditionOperation(Manager<Condition> condition)
        {
            _cond = condition;
        }

        private Manager<Condition> _cond;


        public ConditionConcat Equal(object value)
        {
            if (value != null)
            {
                _cond.i.operation = "=";
                _cond.i.operation_params.Add(value);
            }
            else
            {
                _cond.i.operation = "IS NULL";
            }

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotEqual(object value)
        {
            if (value != null)
            {
                _cond.i.operation = "<>";
                _cond.i.operation_params.Add(value);
            }
            else
            {
                _cond.i.operation = "IS NOT NULL";
            }

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Greater(object value)
        {
            _cond.i.operation = ">";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat GreaterOrEqual(object value)
        {
            _cond.i.operation = ">=";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Less(object value)
        {
            _cond.i.operation = "<";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat LessOrEqual(object value)
        {
            _cond.i.operation = ">=";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Between(object value, object value2)
        {
            _cond.i.operation = "BETWEEN";
            _cond.i.operation_params.Add(value);
            _cond.i.operation_params.Add(value2);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotBetween(object value, object value2)
        {
            _cond.i.operation = "NOT BETWEEN";
            _cond.i.operation_params.Add(value);
            _cond.i.operation_params.Add(value2);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Contains(object value)
        {
            _cond.i.operation = "LIKE";
            _cond.i.operation_params.Add($"%{value}%");

            return new ConditionConcat(_cond);
        }
        public ConditionConcat ContainsCaseInsensitive(object value)
        {
            _cond.i.operation = "LIKE";
            _cond.i.operation_columnFunction = "LOWER";
            _cond.i.operation_params.Add($"%{value}%");

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Like(object value)
        {
            _cond.i.operation = "LIKE";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat LikeCaseInsensitive(object value)
        {
            _cond.i.operation = "LIKE";
            _cond.i.operation_columnFunction = "LOWER";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotLike(object value)
        {
            _cond.i.operation = "NOT LIKE";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotLikeCaseInsensitive(object value)
        {
            _cond.i.operation = "NOT LIKE";
            _cond.i.operation_columnFunction = "LOWER";
            _cond.i.operation_params.Add(value);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat Null()
        {
            _cond.i.operation = "IS NULL";

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotNull()
        {
            _cond.i.operation = "IS NOT NULL";

            return new ConditionConcat(_cond);
        }
        public ConditionConcat In(IEnumerable<object> values)
        {
            _cond.i.operation = "IN";
            _cond.i.operation_params.AddRange(values);

            return new ConditionConcat(_cond);
        }
        public ConditionConcat NotIn(List<object> values)
        {
            _cond.i.operation = "NOT IN";
            _cond.i.operation_params.AddRange(values);

            return new ConditionConcat(_cond);
        }
    }
}