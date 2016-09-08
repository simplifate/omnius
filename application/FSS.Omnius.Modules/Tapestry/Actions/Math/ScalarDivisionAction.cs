using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [MathRepository]
    class ScalarDivisionAction : Action
    {
        public override int Id
        {
            get
            {
                return 4013;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableData", "ColumnName", "Divider", "?TargetColumnName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Math: Scalar division";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            var columnName = (string)vars["ColumnName"];
            var divider = Convert.ToDouble(vars["Divider"]);
            if (vars.ContainsKey("TargetColumnName"))
            {
                string TargetColumnName = (string)vars["TargetColumnName"];
                foreach (var row in tableData)
                {
                    row[TargetColumnName] = Convert.ToDouble(row[columnName]) / divider;
                }
            }
            else
            {
                foreach (var row in tableData)
                {
                    row[columnName] = Convert.ToDouble(row[columnName]) / divider;
                }
            }
            outputVars["Result"] = tableData;
        }
    }
}
