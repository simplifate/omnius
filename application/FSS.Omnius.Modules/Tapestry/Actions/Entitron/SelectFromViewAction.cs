using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectFromViewAction : Action
    {
        public override int Id
        {
            get
            {
                return 1034;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// You can add more conditions by add InputVar "?ColumnNameX" and "?ValueX" where X is number in the row
        /// </summary>
        public override string[] InputVar
        {
            get
            {
                return new string[] { "ViewName", "?SearchInShared", "?ColumnName", "?Value", "?ColumnName2", "?Value2", "?ColumnName3", "?Value3" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select from view";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";

            // get view
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            var view = core.Entitron.GetDynamicView((string)vars["ViewName"], searchInShared);

            var result = view.Select();
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
                // condition is list
                if (!(value is string) && value is IEnumerable)
                {
                    // condition list is empty -> return empty list
                    if (((IEnumerable<object>)value).Count() == 0)
                    {
                        outputVars["Result"] = new List<DBItem>();
                        return;
                    }

                    result = result.where(c => c.column(columnName).In((IEnumerable<object>)value));
                }
                // condition is list of strings
                else if ((value is string) && ((string)value).Contains(","))
                {
                    string[] list = vars["Value"].ToString().Split(',');
                    result = result.where(c => c.column(columnName).In(list));
                }
                // condition is object
                else
                    result = result.where(c => c.column(columnName).Equal(value));
                
                conditionIndex++;
            }

            // result
            outputVars["Result"] = result.ToList();
        }
    }
}
