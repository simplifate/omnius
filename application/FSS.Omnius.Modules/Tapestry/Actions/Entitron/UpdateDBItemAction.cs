using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class UpdateDBItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1007;
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
                return new string[] { "TableName", "Id", "Item[PropertyName]" };
            }
        }

        public override string Name
        {
            get
            {
                return "UpdateDBItem";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;

            string error;
            string tableName = (string)vars["TableName"];
            int? itemId = (int?)vars["Id"];

            if(string.IsNullOrEmpty(tableName)) {
                error = string.Format("Nebyl předán název tabulky (Akce: {0} ({1}))", Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            if(itemId == null) {
                error = string.Format("Nebylo předáno Id záznamu (Akce: {0} ({1}))", Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            DBTable table = ent.GetDynamicTable(tableName);
            if(table == null) {
                error = string.Format("Požadovaná tabulka nebyla nalezena (Tabulka: {0}, Akce: {1} ({2}))", tableName, Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            var select = table.Select();
            Conditions condition = new Conditions(select);
            Condition_concat outCondition = null;

            outCondition = condition.column("Id").Equal(itemId);
            condition = outCondition.and();

            DBItem item = table.Select().where(i => outCondition).First();
            if(item == null) {
                error = string.Format("Položka nebyla nalezena (Tabulka: {0}, Id: {1}, Akce: {2} ({3}))", tableName, itemId, Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            var propertyNames = vars.Keys.Where(k => k.StartsWith("Item[") && k.EndsWith("]"));
            foreach (string propertyName in propertyNames)
            {
                string itemProperty = propertyName.Substring(5, propertyName.Length - 6);
                item[itemProperty] = vars[propertyName];
            }

            table.Update(item, item);
            ent.Application.SaveChanges();
        }
    }
}
