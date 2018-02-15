using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Math
{
    [MathRepository]
    class CopyInVectorAction : Action
    {
        public override int Id => 4014;

        public override string[] InputVar => new string[] { "TableData", "SourceColumn", "TargetColumn", "?ConstantSource" };

        public override string Name => "Math: Copy in vector";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            string sourceColumn = (string)vars["SourceColumn"];
            string targetColumn = (string)vars["TargetColumn"];
            if (tableData.Count == 0)
                return;
            var firstRow = tableData[0];

            if (vars.ContainsKey("ConstantSource"))
            {
                object constant = vars["ConstantSource"];

                foreach (var row in tableData)
                {
                    row[targetColumn] = constant;
                }
            }
            else
            {
                foreach (var row in tableData)
                {
                    row[targetColumn] = row[sourceColumn];
                }
            }
            outputVars["Result"] = tableData;
        }
    }
}
