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
                return new string[] { "ItemId", "TableName" };
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
            var itemId = Convertor.convert('i', core._form["Id"]);
            string tableName = (string)vars["TableName"];
            string error;

            if (string.IsNullOrEmpty(tableName))
            {
                error = string.Format("Nebyl předán název tabulky (Akce: {0} ({1}))", Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            if (itemId == null)
            {
                error = string.Format("Nebylo předáno Id záznamu (Akce: {0} ({1}))", Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            DBTable table = ent.GetDynamicTable(tableName);
            if (table == null)
            {
                error = string.Format("Požadovaná tabulka nebyla nalezena (Tabulka: {0}, Akce: {1} ({2}))", tableName, Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }
            
            table.Remove((int)itemId);
            ent.Application.SaveChanges();
        }
    }
}
