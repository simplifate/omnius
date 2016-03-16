using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class UpdateDBModelAction : Action
    {
        public override int Id
        {
            get
            {
                return 1002;
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
                return new string[] { "Id" };
            }
        }

        public override string Name
        {
            get
            {
                return "Update Model";
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
            DBEntities e = new DBEntities();

            if (!vars.ContainsKey("Id"))
            {
                string error = string.Format("Nebylo předáno Id záznamu (Akce: {0} ({1}))", Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            string tableName = (string)vars["__TableName__"];
            int itemId = (int)vars["Id"];
            DBTable table = ent.GetDynamicTable(tableName);
            if(table == null) {
                string error = string.Format("Požadovaná tabulka nebyla nalezena (Tabulka: {0}, Akce: {1} ({2}))", tableName, Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            DBItem row = table.Select().where(c => c.column("Id").Equal(itemId)).First();
            if (row == null) {
                string error = string.Format("Položka nebyla nalezena (Tabulka: {0}, Id: {1}, Akce: {2} ({3}))", tableName, itemId, Name, Id);
                LogError(error, core.User.Id, ent.AppId);
                throw new Exception(error);
            }

            foreach (DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    row[column.Name] = vars.ContainsKey($"__Model.{tableName}.{column.Name}");
                else if (vars.ContainsKey($"__Model.{tableName}.{column.Name}"))
                    row[column.Name] = vars[$"__Model.{tableName}.{column.Name}"];
            }

            table.Update(row, itemId);
            ent.Application.SaveChanges();
        }
    }
}
