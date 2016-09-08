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
                return new string[] { "?TableName", "?Id" };
            }
        }

        public override string Name
        {
            get
            {
                return "Update DB Item";
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
            DBEntities e = DBEntities.instance;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            int itemId = vars.ContainsKey("Id")
                ? (int)vars["Id"]
                : (int)vars["__ModelId__"];
            DBTable table = ent.GetDynamicTable(tableName);
            if(table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem row = table.Select().where(c => c.column("Id").Equal(itemId)).First();
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {tableName}, Id: {itemId}, Akce: {Name} ({Id}))");

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
