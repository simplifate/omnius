using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GatherStringsAction : Action
    {
        public override int Id => 191;

        public override string[] InputVar => new string[] { "TableData", "ColumnName" };

        public override string Name => "Gather emails to single string";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            var columnName = (string)vars["ColumnName"];
            if (tableData.Count == 0)
            {
                outputVars["Result"] = "";
            }
            else
            {
                List<string> listStrings = new List<string>();
                foreach (var row in tableData)
                {
                    listStrings.Add((string)row[columnName]);
                }

                // gather strings and divide them by "," to single string output
                outputVars["Result"] = string.Join(",", listStrings);
            }
        }
    }
}
