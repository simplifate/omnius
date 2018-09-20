using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SearchInTableAction : Action
    {
        public override int Id => 1026;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "ColumnName", "Query", "?SearchMode", "?SearchInShared" };

        public override string Name => "Search in table";

        public override string[] OutputVar => new string[] { "Data" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            DBConnection db = COREobject.i.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string query = "";
            if (vars.ContainsKey("SearchMode"))
            {
                switch((string)vars["SearchMode"])
                {
                    case "start":
                    default:
                        query = (string)vars["Query"] + "%";
                        break;
                    case "end":
                        query = "%" + (string)vars["Query"];
                        break;
                    case "anywhere":
                        query = "%" + (string)vars["Query"] + "%";
                        break;
                }
            }
            else
                query = (string)vars["Query"] + "%";

            outputVars["Data"] = db.Select((string)vars["TableName"], searchInShared)
                .Where(c => c.Column((string)vars["ColumnName"]).LikeCaseInsensitive(query)).ToList();
        }
    }
}
