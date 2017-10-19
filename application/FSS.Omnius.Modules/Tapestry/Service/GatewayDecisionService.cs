using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Tapestry.Service
{
    public class GatewayDecisionService
    {
        public static bool MatchConditionSets(ICollection<TapestryDesignerConditionSet> conditionSets, Dictionary<string, object> vars)
        {
            bool result = true;
            foreach (var conditionSet in conditionSets)
            {
                if (conditionSet.SetRelation == "AND")
                    result = result && matchConditionSet(conditionSet, vars);
                else if (conditionSet.SetRelation == "OR")
                    result = result || matchConditionSet(conditionSet, vars);
            }
            return result;
        }
        private static bool matchConditionSet(TapestryDesignerConditionSet conditionSet, Dictionary<string, object> vars)
        {
            bool result = true;
            foreach (var condition in conditionSet.Conditions)
            {
                if (condition.Relation == "AND")
                    result = result && matchCondition(condition, vars);
                else if (condition.Relation == "OR")
                    result = result || matchCondition(condition, vars);
            }
            return result;
        }
        private static bool matchCondition(TapestryDesignerCondition condition, Dictionary<string, object> vars)
        {
            object leftOperand = KeyValueString.ParseValue(condition.Variable, vars);
            switch (condition.Operator)
            {
                case "is empty":
                    return leftOperand == null || leftOperand == DBNull.Value || leftOperand.ToString().Length == 0;
                case "is not empty":
                    return leftOperand != null && leftOperand != DBNull.Value && leftOperand.ToString().Length > 0;
            }

            object value = KeyValueString.ParseValue(condition.Value, vars);
            if (leftOperand == null || value == null)
                return false;
            switch (condition.Operator)
            {
                case "==":
                    if (leftOperand is bool)
                        return (bool)leftOperand == (bool)value;
                    else
                        return leftOperand.ToString() == value.ToString();
                case "!=":
                    if (leftOperand is bool)
                        return (bool)leftOperand != (bool)value;
                    else
                        return leftOperand.ToString() != value.ToString();
                case ">":
                    if (leftOperand is DateTime && value is DateTime)
                        return (DateTime)leftOperand > (DateTime)value;
                    else
                        return Convert.ToDecimal(leftOperand) > Convert.ToDecimal(value);
                case ">=":
                    if (leftOperand is DateTime && value is DateTime)
                        return (DateTime)leftOperand >= (DateTime)value;
                    else
                        return Convert.ToDecimal(leftOperand) >= Convert.ToDecimal(value);
                case "<":
                    if (leftOperand is DateTime && value is DateTime)
                        return (DateTime)leftOperand < (DateTime)value;
                    else
                        return Convert.ToDecimal(leftOperand) < Convert.ToDecimal(value);
                case "<=":
                    if (leftOperand is DateTime && value is DateTime)
                        return (DateTime)leftOperand <= (DateTime)value;
                    else
                        return Convert.ToDecimal(leftOperand) <= Convert.ToDecimal(value);
                case "contains":
                    return ((string)leftOperand).Contains(value.ToString());
                case "is in":
                    return ((List<object>)value).Contains(leftOperand);
            }
            return true;

        }
    }
}
