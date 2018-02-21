using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SetStateAction : Action
    {
        public override int Id => 1029;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "ColumnName", "StateId", "?RowId", "?SearchInShared" };

        public override string Name => "Set state";

        public override string[] OutputVar => new string[0];

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            if ((vars.ContainsKey("__TableName__") || (vars.ContainsKey("TableName"))) && (vars.ContainsKey("__ModelId__") || vars.ContainsKey("RowId")))
            {
                int rowId = vars.ContainsKey("RowId") ? Convert.ToInt32(vars["RowId"]) : (int)vars["__ModelId__"];
                DBItem model = vars.ContainsKey("TableName")
                    ? db.Tabloid((string)vars["TableName"], searchInShared).SelectById(rowId)
                    : db.Tabloid((string)vars["__TableName__"], searchInShared).SelectById(rowId);
                model[(string)vars["ColumnName"]] = (int)vars["StateId"];
                (model.Tabloid as DBTable).Update(model, rowId);
                db.SaveChanges();
            }
            else
            {
                DBItem model = (DBItem)vars["__MODEL__"];
                model[(string)vars["ColumnName"]] = (int)vars["StateId"];
            }
        }
    }
}
