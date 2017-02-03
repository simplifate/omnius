using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
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
        public override int Id
        {
            get
            {
                return 1010;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return 1004;

            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?ItemId", "?TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Delete Item";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;

            var itemId = vars.ContainsKey("ItemId")
                ? (int)vars["ItemId"]
                : (vars.ContainsKey("deleteId") ? (int)vars["deleteId"] : (int)vars["__ModelId__"]);
            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = ent.GetDynamicTable(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");
            
            table.Remove(itemId);
            ent.Application.SaveChanges();
        }
    }
}
