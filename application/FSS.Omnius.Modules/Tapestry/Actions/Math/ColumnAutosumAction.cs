﻿using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class ColumnAutosumAction : Action
    {
        public override int Id => 4005;

        public override string[] InputVar => new string[] { "TableData", "ColumnName" };

        public override string Name => "Math: Column autosum";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            var columnName = (string)vars["ColumnName"];
            if (tableData.Count == 0)
            {
                outputVars["Result"] = 0;
            }
            else if (tableData[0][columnName] is int)
            {
                int sum = 0;
                foreach(var row in tableData)
                {
                    sum += (int)row[columnName];
                }
                outputVars["Result"] = sum;
            }
            else
            {
                double sum = 0;
                foreach (var row in tableData)
                {
                    sum += Convert.ToDouble(row[columnName]);
                }
                outputVars["Result"] = sum;
            }
        }
    }
}
