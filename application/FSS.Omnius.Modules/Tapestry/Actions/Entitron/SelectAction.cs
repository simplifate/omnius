using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectAction : Action
    {
        public override int Id
        {
            get
            {
                return 1020;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "?SearchInShared", "?Top", "?OrderBy", "?Descending", "?GroupBy", "?GroupByFunction", "CondColumn[index]", "CondValue[index]", "?CondOperator[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select (filter)";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            // init
            DBConnection db = Modules.Entitron.Entitron.i;

            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"];
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string orderBy = vars.ContainsKey("OrderBy") ? (string)vars["OrderBy"] : null;
            AscDesc isDescending = vars.ContainsKey("Descending") && (bool)vars["Descending"] ? AscDesc.Desc : AscDesc.Asc;

            //
            Tabloid tabloid = db.Tabloid(tableName, searchInShared);
            Select select = tabloid.Select();
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                string condOperator = vars.ContainsKey($"CondOperator[{i}]") ? (string)vars[$"CondOperator[{i}]"] : "Equal";
                string condColumn = (string)vars[$"CondColumn[{i}]"];
                object condValue = vars[$"CondValue[{i}]"];

                DBColumn column = tabloid.Columns.Single(c => c.Name == condColumn);
                object value = condOperator != "IsIn"
                    ? DataType.ConvertTo(column.Type, condValue)
                    : condValue;

                switch (condOperator)
                {
                    case "Less":
                        select.Where(c => c.Column(condColumn).Less(value));
                        break;
                    case "LessOrEqual":
                        select.Where(c => c.Column(condColumn).LessOrEqual(value));
                        break;
                    case "Greater":
                        select.Where(c => c.Column(condColumn).Greater(value));
                        break;
                    case "GreaterOrEqual":
                        select.Where(c => c.Column(condColumn).GreaterOrEqual(value));
                        break;
                    case "Equal":
                        select.Where(c => c.Column(condColumn).Equal(value));
                        break;
                    case "IsIn":
                        // string, multiple values
                        if ((condValue is string) && ((string)condValue).Contains(","))
                            select.Where(c => c.Column(condColumn).In((condValue as string).Split(',')));
                        // Enumerable
                        else
                            select.Where(c => c.Column(condColumn).In((IEnumerable<object>)condValue));
                        break;
                    default: // ==
                        select.Where(c => c.Column(condColumn).Equal(value));
                        break;
                }
            }

            // top
            if (vars.ContainsKey("Top"))
                select = select.Limit((int)vars["Top"]);

            // order
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                select = select.Order(isDescending, orderBy);

                if (vars.ContainsKey("MaxRows"))
                    select.DropStep((int)vars["MaxRows"], ESqlFunction.LAST, isDescending, orderBy);
            }

            // Group
            if (vars.ContainsKey("GroupBy") && !string.IsNullOrWhiteSpace((string)vars["GroupBy"]))
            {
                ESqlFunction function = ESqlFunction.SUM;
                if (vars.ContainsKey("GroupByFunction"))
                {
                    switch ((string)vars["GroupByFunction"])
                    {
                        case "none":
                            function = ESqlFunction.none;
                            break;
                        case "MAX":
                            function = ESqlFunction.MAX;
                            break;
                        case "MIN":
                            function = ESqlFunction.MIN;
                            break;
                        case "AVG":
                            function = ESqlFunction.AVG;
                            break;
                        case "COUNT":
                            function = ESqlFunction.COUNT;
                            break;
                        case "SUM":
                            function = ESqlFunction.SUM;
                            break;
                        case "FIRST":
                            function = ESqlFunction.FIRST;
                            break;
                        case "LAST":
                            function = ESqlFunction.LAST;
                            break;
                    }
                }
                select.Group(function, columns: (string)vars["GroupBy"]);
            }

            // return
            outputVars["Data"] = select.ToList();
        }
    }
}
