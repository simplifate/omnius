using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class SubtractInVectorAction : Action
    {
        public override int Id => 4010;

        public override string[] InputVar => new string[] { "TableData", "ColumnA", "ColumnB", "ResultColumn" };

        public override string Name => "Math: Subtract in vector";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            var columnA = (string)vars["ColumnA"];
            var columnB = (string)vars["ColumnB"];
            var resultColumn = (string)vars["ResultColumn"];

            if(tableData.Count == 0)
                return;
            var firstRow = tableData[0];
            bool integerMode = firstRow[columnA] is int && firstRow[columnB] is int;
            
            if (integerMode)
            {
                foreach (var row in tableData)
                {
                    row[resultColumn] = (int)row[columnA] - (int)row[columnB];
                }
            }
            else
            {
                foreach (var row in tableData)
                {
                    row[resultColumn] = Convert.ToDouble(row[columnA]) - Convert.ToDouble(row[columnB]);
                }
            }
            outputVars["Result"] = tableData;
        }
    }
}
