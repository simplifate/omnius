using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDbItemAction : Action
    {
        public override int Id
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
                return new string[] { "?TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Create DB Item";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return 1010;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = core.Entitron.GetDynamicTable(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem item = new DBItem();
            foreach (DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    item.createProperty(column.ColumnId, column.Name, vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"));
                else if (column.type == "datetime")
                    item.createProperty(column.ColumnId, column.Name, DateTime.Parse((string)vars[$"__Model.{table.tableName}.{column.Name}"]));
                else if (vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"))
                    item.createProperty(column.ColumnId, column.Name, vars[$"__Model.{table.tableName}.{column.Name}"]);
            }

            table.Add(item);
            core.Entitron.Application.SaveChanges();
        }
    }
}
