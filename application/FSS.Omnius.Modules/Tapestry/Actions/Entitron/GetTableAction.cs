using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetTableAction : Action
    {
        public override int Id => 1003;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "?SearchInShared" };

        public override string Name => "Get Table";

        public override string[] OutputVar => new string[] { "Data", "columnNames" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            DBTable table = db.Table(vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"], searchInShared);

            outputVars["columnNames"] = table.Columns.Select(c => c.Name);
            outputVars["Data"] = table.Select().ToList();
        }
    }
}
