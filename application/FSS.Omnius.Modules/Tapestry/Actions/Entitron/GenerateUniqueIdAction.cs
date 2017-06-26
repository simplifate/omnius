using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GenerateUniqueIdAction : Action
    {
        public override int Id
        {
            get
            {
                return 1048;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "ColumnName", "?SearchInShared", "?NumericalSeries" };
            }
        }

        public override string Name
        {
            get
            {
                return "Generate unique ID";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string numSeries = vars.ContainsKey("NumericalSeries") ? (string)vars["NumericalSeries"] : null;
            string tableName = (string)vars["TableName"];
            string columnName = (string)vars["ColumnName"];

            var table = core.Entitron.GetDynamicTable(tableName, searchInShared);
            var results = table.Select().ToList();
            var notNullResults = results.Select(c => c[columnName]).Where(c => c != DBNull.Value).Select(c => (int)c).ToList();
            if (numSeries == null)
            {
                int prevoiusId = notNullResults.Count > 0 ? notNullResults.Max() : 0;   
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
                        previousId = notNullResults.Count > 0 ? notNullResults.Where(c => c < 6000).Max() : 999;
                        previousId = previousId < 999 ? 999 : previousId;
                        break;
                    case "B": //8000+
                    case "Z":
                        previousId = notNullResults.Count > 0 ? notNullResults.Max() : 7999;
                        previousId = previousId < 7999 ? 7999 : previousId;
                        break;
                    case "F": //6000+
                        previousId = notNullResults.Count > 0 ? notNullResults.Where(c => c < 7000).Max() : 5999;
                        previousId = previousId < 5999 ? 5999 : previousId;
                        break;
                    case "S": //7000+
                        previousId = notNullResults.Count > 0 ? notNullResults.Where(c => c < 8000).Max() : 6999;
                        previousId = previousId < 6999 ? 6999 : previousId;
                        break;
                    default:
                        previousId = notNullResults.Count > 0 ? notNullResults.Max() : 0;
                        break;
                }
                outputVars["Result"] = previousId + 1;

            }
        }
    }
}
