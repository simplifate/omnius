using FSS.Omnius.Modules.Tapestry2.Services;
using System;
using System.Linq;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public partial class TapestryDesignerConditionGroup
    {
        public string ToString(TapestryGenerateService tapestryGenerate)
        {
            StringBuilder result = new StringBuilder();
            bool isFirst = true;
            foreach (TapestryDesignerConditionSet conditionSet in ConditionSets.OrderBy(cs => cs.SetIndex))
            {
                if (!isFirst)
                    result.Append(conditionSet.SetRelation == "AND" ? " && " : " || ");

                result.Append(conditionSet.ToString(tapestryGenerate));
                isFirst = false;
            }

            return result.ToString();
        }
    }

    public partial class TapestryDesignerConditionSet
    {
        public string ToString(TapestryGenerateService tapestryGenerate)
        {
            StringBuilder result = new StringBuilder();
            result.Append("(");

            bool isFirst = true;
            foreach (TapestryDesignerCondition condition in Conditions.OrderBy(c => c.Index))
            {
                if (!isFirst)
                    result.Append(condition.Relation == "AND" ? " && " : " || ");

                result.Append(condition.ToString(tapestryGenerate));
                isFirst = false;
            }

            result.Append(")");
            return result.ToString();
        }
    }

    public partial class TapestryDesignerCondition
    {
        public string ToString(TapestryGenerateService tapestryGenerate)
        {
            string resultValue = tapestryGenerate.ForInput(Value, null);
            string variable = tapestryGenerate.ForInput(Variable, null);

            switch (Operator)
            {
                case "exists":
                    return $"({variable} != null)";
                case "is empty":
                    return $"(Extend.IsEmpty({variable}))";
                case "is not empty":
                    return $"(!Extend.IsEmpty({variable}))";
                case "is in":
                    return $"(({resultValue} as IEnumerable<dynamic>).Contains({variable}))";
                case "contains":
                    return $"(({variable} as IEnumerable<dynamic>).Contains({resultValue}))";
                case "==":
                    return $"(Extend.Compare({variable}, {resultValue}))";
                case "!=":
                    return $"(!Extend.Compare({variable}, {resultValue}))";
                // < <= > >=
                case "<":
                    return $"(Extend.LessThan({variable}, {resultValue}))";
                case "<=":
                    return $"(Extend.LessEqThan({variable}, {resultValue}))";
                case ">":
                    return $"(Extend.GreaterThan({variable}, {resultValue}))";
                case ">=":
                    return $"(Extend.GreaterEqThan({variable}, {resultValue}))";
                default:
                    return $"({variable} {Operator} {resultValue})";
            }
        }
    }
}
