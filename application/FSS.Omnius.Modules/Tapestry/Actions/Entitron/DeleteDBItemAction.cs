using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class DeleteDBItemAction : Action
    {
        public override int Id => 1010;

        public override int? ReverseActionId => 1004;

        public override string[] InputVar => new string[] { "?ItemId", "?TableName", "?SearchInShared" };

        public override string Name => "Delete Item";

        public override string[] OutputVar => new string[0];

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = COREobject.i.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            var itemId = vars.ContainsKey("ItemId")
                ? (vars["ItemId"] is int ? (int)vars["ItemId"] : Convert.ToInt32(vars["ItemId"]))
                : (vars.ContainsKey("deleteId") ? Convert.ToInt32(vars["deleteId"]) : (int)vars["__ModelId__"]);
            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = db.Table(tableName, searchInShared);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");
            
            table.Delete(itemId);
            db.SaveChanges();
        }
    }
}
