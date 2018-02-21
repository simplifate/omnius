using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GenerateUniqueIdAction : Action
    {
        public override int Id => 1048;

        public override string[] InputVar => new string[] { "TableName", "ColumnName", "?SearchInShared", "?NumericalSeries" };

        public override string Name => "Generate unique ID";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string numSeries = vars.ContainsKey("NumericalSeries") ? (string)vars["NumericalSeries"] : null;
            string tableName = (string)vars["TableName"];
            string columnName = (string)vars["ColumnName"];

            DBTable table = db.Table(tableName, searchInShared);
            var results = table.Select(columnName).Where(c => c.Column(columnName).NotNull()).ToList().Select(c => (int)c[columnName]).ToList();
            if (numSeries == null)
            {
                int prevoiusId = results.Count > 0 ? results.Max() : 0;   
                outputVars["Result"] = prevoiusId + 1;
            }
            else
            {
                int previousId;
                numSeries = numSeries.ToUpper();
                
                switch (numSeries)
                {
                    case "A": //1000+
                    case "C":
                    case "E":
                    case "P":
                        previousId = results.Count > 0 ? results.Where(c => c < 6000).Max() : 999;
                        previousId = previousId < 999 ? 999 : previousId;
                        break;
                    case "B": //8000+
                    case "Z":
                        previousId = results.Count > 0 ? results.Max() : 7999;
                        previousId = previousId < 7999 ? 7999 : previousId;
                        break;
                    case "F": //6000+
                        previousId = results.Count > 0 ? results.Where(c => c < 7000).Max() : 5999;
                        previousId = previousId < 5999 ? 5999 : previousId;
                        break;
                    case "S": //7000+
                        previousId = results.Count > 0 ? results.Where(c => c < 8000).Max() : 6999;
                        previousId = previousId < 6999 ? 6999 : previousId;
                        break;
                    default:
                        previousId = results.Count > 0 ? results.Max() : 0;
                        break;
                }
                outputVars["Result"] = previousId + 1;
            }
        }
    }
}
