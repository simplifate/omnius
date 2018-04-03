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
    public class MassDeleteDBItemAction : Action
    {
        public override int Id => 1010121;

        public override int? ReverseActionId => 1004; //not sure .. need consult

        public override string[] InputVar => new string[] { "?ItemList", "?TableName", "?SearchInShared" };

        public override string Name => "Mass Delete DBItem";

        public override string[] OutputVar => new string[0];

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;


            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = db.Table(tableName, searchInShared);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            List<DBItem> deleteItemList = (List<DBItem>)vars["ItemList"];
            foreach (DBItem item in deleteItemList)
            {
                table.Delete(item);
            }

            db.SaveChanges();
        }
    }
}
