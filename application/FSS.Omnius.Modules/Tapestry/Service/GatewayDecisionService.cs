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
            if (!vars.ContainsKey(condition.Variable))
                return false;

            var leftOperand = vars[condition.Variable];
            switch (condition.Operator)
            {
                case "==":
                    if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") == condition.Value;
                    else
                        return leftOperand.ToString() == condition.Value.ToString();
                case "!=":
                    if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") != condition.Value;
                    else
                        return leftOperand.ToString() == condition.Value.ToString();
                case ">":
                    return Convert.ToInt64(leftOperand) > Convert.ToInt64(condition.Value);
                case ">=":
                    return Convert.ToInt64(leftOperand) >= Convert.ToInt64(condition.Value);
                case "<":
                    return Convert.ToInt64(leftOperand) < Convert.ToInt64(condition.Value);
                case "<=":
                    return Convert.ToInt64(leftOperand) <= Convert.ToInt64(condition.Value);
                case "is empty":
                    return string.IsNullOrEmpty((string)leftOperand);
                case "is not empty":
                    return !string.IsNullOrEmpty((string)leftOperand);
                case "contains":
                    return ((string)leftOperand).Contains(condition.Value);
            }
            return true;
        }
    }
}
