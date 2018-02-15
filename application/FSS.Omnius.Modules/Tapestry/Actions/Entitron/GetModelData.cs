using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetModelData : Action
    {
        public override int Id => 1002;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "ColumnName", "?SearchInShared" };

        public override string Name => "Get Model Data";

        public override string[] OutputVar => new string[] { "Data" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            DBItem model = db.Table((string)vars["__TableName__"], searchInShared).SelectById((int)vars["__ModelId__"]);
            if (vars.ContainsKey("ColumnName"))
                outputVars["Data"] = model[(string)vars["ColumnName"]];
            else
                outputVars["Data"] = model;
        }
    }
}
