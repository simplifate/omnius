using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectFromViewAction : Action
    {
        public override int Id => 1034;

        public override int? ReverseActionId => null;

        /// <summary>
        /// You can add more conditions by add InputVar "?ColumnNameX" and "?ValueX" where X is number in the row
        /// </summary>
        public override string[] InputVar => new string[] { "ViewName", "?SearchInShared", "?OrderBy", "?Descending", "?GroupBy", "?GroupByFunction", "?MaxRows", "?ColumnName", "?Value", "?ColumnName2", "?Value2", "?ColumnName3", "?Value3" };

        public override string Name => "Select from view";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            DBConnection db = COREobject.i.Entitron;

            // get view
            string orderBy = vars.ContainsKey("OrderBy") ? (string)vars["OrderBy"] : null;
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            AscDesc isDescending = vars.ContainsKey("Descending") && (bool)vars["Descending"] ? AscDesc.Desc : AscDesc.Asc;

            Tabloid view = db.Tabloid((string)vars["ViewName"], searchInShared);

            var select = view.Select();
            int conditionIndex = 1;
            /// each condition
            while (true)
            {
                string postfix = (conditionIndex == 1 ? "" : conditionIndex.ToString());
                string columnNameVars = $"ColumnName{postfix}";
                string valueVars = $"Value{postfix}";

                // select all - no condition
                if (!vars.ContainsKey(columnNameVars) || !vars.ContainsKey(valueVars))
                    break;

                string columnName = (string)vars[columnNameVars];
                object value = vars[valueVars];
                if (value is Newtonsoft.Json.Linq.JValue)
                    select.Where(c => c.Column(columnName).Equal(value.ToString()));
                // condition is list
                else if (!(value is string) && value is IEnumerable)
                {
                    // condition list is empty -> return empty list
                    if (((IEnumerable<object>)value).Count() == 0)
                    {
                        outputVars["Result"] = new List<DBItem>();
                        return;
                    }

                    select.Where(c => c.Column(columnName).In((IEnumerable<object>)value));
                }
                // condition is list of strings
                else if ((value is string) && ((string)value).Contains(","))
                {
                    string[] list = vars["Value"].ToString().Split(',');
                    select.Where(c => c.Column(columnName).In(list));
                }
                // condition is object
                else
                    select.Where(c => c.Column(columnName).Equal(value));
                
                conditionIndex++;
            }

            // MaxRows
            if (vars.ContainsKey("MaxRows") && orderBy != null)
                select.DropStep((int)vars["MaxRows"], ESqlFunction.LAST, isDescending, orderBy);

            // order
            select.Order(isDescending, orderBy);

            // group
            if (vars.ContainsKey("GroupBy") && !string.IsNullOrWhiteSpace((string)vars["GroupBy"]))
            {
                ESqlFunction function = ESqlFunction.SUM;
                if (vars.ContainsKey("GroupByFunction"))
                {
                    switch((string)vars["GroupByFunction"])
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
            
            outputVars["Result"] = select.ToList();
        }
    }
}
