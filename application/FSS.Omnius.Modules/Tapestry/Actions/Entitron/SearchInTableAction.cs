using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SearchInTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1026;
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
                return new string[] { "TableName", "ColumnName", "Query", "?SearchMode", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Search in table";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";
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

            outputVars["Data"] = core.Entitron.GetDynamicTable((string)vars["TableName"], searchInShared).Select()
                .where(c => c.column((string)vars["ColumnName"]).LikeCaseInsensitive(query)).ToList();
        }
    }
}
