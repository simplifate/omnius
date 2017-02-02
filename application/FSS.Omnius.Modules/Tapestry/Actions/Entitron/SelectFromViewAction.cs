using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System.Collections;
using System.Collections.Generic;

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
        public override string[] InputVar
        {
            get
            {
                return new string[] { "ViewName", "ColumnName", "Value", "?ColumnName2", "?Value2" };
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

            var view = core.Entitron.GetDynamicView((string)vars["ViewName"]);

            if (vars.ContainsKey("ColumnName") && !(vars["Value"] is string) && vars["Value"] is IEnumerable && ((List<object>)vars["Value"]).Count == 0)
            {
                outputVars["Result"] = new List<DBItem>();
                return;
            }
            if (vars.ContainsKey("ColumnName2") && !(vars["Value2"] is string) && vars["Value2"] is IEnumerable && ((List<object>)vars["Value2"]).Count == 0)
            {
                outputVars["Result"] = new List<DBItem>();

                return;
            }

            if (!vars.ContainsKey("ColumnName"))
            {
                outputVars["Result"] = view.Select().ToList();
            }
            else if (vars.ContainsKey("ColumnName2"))
            {
                string columnName = (string)vars["ColumnName"];
                string columnName2 = (string)vars["ColumnName2"];
                if ((vars["Value"] is IEnumerable && !(vars["Value"] is string)) && (vars["Value2"] is IEnumerable && !(vars["Value2"] is string)))
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).In((List<object>)vars["Value"]).and().column(columnName2).In((List<object>)vars["Value2"])).ToList();
                }
                else if ((vars["Value"] is IEnumerable && !(vars["Value"] is string)) && !((vars["Value2"] is IEnumerable && !(vars["Value2"] is string))))
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).In((List<object>)vars["Value"]).and().column(columnName2).Equal(vars["Value2"])).ToList();
                }
                else if (!((vars["Value"] is IEnumerable && !(vars["Value"] is string))) && (vars["Value2"] is IEnumerable && !(vars["Value2"] is string)))
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"]).and().column(columnName2).In((List<object>)vars["Value2"])).ToList();
                }
                else
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"]).and().column(columnName2).Equal(vars["Value2"])).ToList();
                }
            }
            else
            {
                string columnName = (string)vars["ColumnName"];
                if (vars["Value"] is IEnumerable && !(vars["Value"] is string))
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).In((List<object>)vars["Value"])).ToList();
                }
                else
                {
                    outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"])).ToList();
                    var varValue = vars["Value"];
                    if (varValue.ToString().Contains(","))
                    {
                        var x = vars["Value"].ToString().Split(',');
                        outputVars["Result"] = view.Select().where(c => c.column(columnName).In((IEnumerable<object>)x)).ToList();
                    }
                    else
                    {
                        outputVars["Result"] = view.Select().where(c => c.column(columnName).Equal(vars["Value"])).ToList();
                    }
                }
            }
        }
    }
}
