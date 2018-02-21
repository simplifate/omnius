using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class ConditionalFilteringService : IConditionalFilteringService
    {
        public bool MatchConditionSets(ICollection<TapestryDesignerConditionSet> conditionSets, DBItem entitronRow, Dictionary<string, object> tapestryVars)
        {
            bool result = true;
            foreach (var conditionSet in conditionSets)
            {
                if (conditionSet.SetRelation == "AND")
                    result = result && matchConditionSet(conditionSet, entitronRow, tapestryVars);
                else if (conditionSet.SetRelation == "OR")
                    result = result || matchConditionSet(conditionSet, entitronRow, tapestryVars);
            }
            return result;
        }
        private bool matchConditionSet(TapestryDesignerConditionSet conditionSet, DBItem entitronRow, Dictionary<string, object> tapestryVars)
        {
            bool result = true;
            foreach (var condition in conditionSet.Conditions)
            {
                if (condition.Relation == "AND")
                    result = result && matchCondition(condition, entitronRow, tapestryVars);
                else if (condition.Relation == "OR")
                    result = result || matchCondition(condition, entitronRow, tapestryVars);
            }
            return result;
        }
        private bool matchCondition(TapestryDesignerCondition condition, DBItem entitronRow, Dictionary<string, object> tapestryVars)
        {
            object leftOperand;
            if (condition.Variable[0] == '#')
            {
                var parsedObjectLeft = KeyValueString.ParseValue(condition.Variable.Substring(1), tapestryVars);
                if (parsedObjectLeft == null)
                    return false;
                leftOperand = parsedObjectLeft;
            }
            else
                leftOperand = entitronRow[condition.Variable];

            var parsedObjectRight = KeyValueString.ParseValue(condition.Value, tapestryVars);
            if (parsedObjectRight == null)
                return false;
            object value = parsedObjectRight;
            switch (condition.Operator)
            {
                case "==":
                    if (leftOperand is bool && value is bool)
                        return (bool)leftOperand == (bool)value;
                    else if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") == value.ToString();
                    else
                        return leftOperand.ToString() == value.ToString();
                case "!=":
                    if (leftOperand is bool && value is bool)
                        return (bool)leftOperand != (bool)value;
                    else if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") != value.ToString();
                    else
                        return leftOperand.ToString() != value.ToString();
                case ">":
                    return Convert.ToInt64(leftOperand) > Convert.ToInt64(value);
                case ">=":
                    return Convert.ToInt64(leftOperand) >= Convert.ToInt64(value);
                case "<":
                    return Convert.ToInt64(leftOperand) < Convert.ToInt64(value);
                case "<=":
                    return Convert.ToInt64(leftOperand) <= Convert.ToInt64(value);
                case "is empty":
                    return value.ToString().Length == 0;
                case "is not empty":
                    return value.ToString().Length > 0;
                case "contains":
                    return ((string)leftOperand).Contains(value.ToString());
            }
            return true;
        }
    }
}
