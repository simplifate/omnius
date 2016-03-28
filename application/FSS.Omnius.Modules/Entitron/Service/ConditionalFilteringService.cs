using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class ConditionalFilteringService : IConditionalFilteringService
    {
        public bool MatchConditionSets(ICollection<TapestryDesignerConditionSet> conditionSets, DBItem entitronRow)
        {
            bool result = true;
            foreach (var conditionSet in conditionSets)
            {
                if (conditionSet.SetRelation == "AND")
                    result = result && matchConditionSet(conditionSet, entitronRow);
                else if (conditionSet.SetRelation == "OR")
                    result = result || matchConditionSet(conditionSet, entitronRow);
            }
            return result;
        }
        private bool matchConditionSet(TapestryDesignerConditionSet conditionSet, DBItem entitronRow)
        {
            bool result = true;
            foreach(var condition in conditionSet.Conditions)
            {
                if (condition.Relation == "AND")
                    result = result && matchCondition(condition, entitronRow);
                else if(condition.Relation == "OR")
                    result = result || matchCondition(condition, entitronRow);
            }
            return result;
        }
        private bool matchCondition(TapestryDesignerCondition condition, DBItem entitronRow)
        {
            var leftOperand = entitronRow[condition.Variable];
            switch (condition.Operator)
            {
                case "==":
                    if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") == condition.Value;
                    else
                        return (string)leftOperand == condition.Value;
                case "!=":
                    if (leftOperand is bool)
                        return ((bool)leftOperand ? "true" : "false") != condition.Value;
                    else
                        return (string)leftOperand != condition.Value;
                case ">":
                    return Convert.ToInt64(leftOperand) > Convert.ToInt64(condition.Value);
                case ">=":
                    return Convert.ToInt64(leftOperand) >= Convert.ToInt64(condition.Value);
                case "<":
                    return Convert.ToInt64(leftOperand) < Convert.ToInt64(condition.Value);
                case "<=":
                    return Convert.ToInt64(leftOperand) <= Convert.ToInt64(condition.Value);
                case "is empty":
                    return condition.Value.Length == 0;
                case "is not empty":
                    return condition.Value.Length > 0;
                case "contains":
                    return ((string)leftOperand).Contains(condition.Value);
            }
            return true;
        }
    }
}
