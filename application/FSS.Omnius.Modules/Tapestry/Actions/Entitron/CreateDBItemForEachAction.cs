using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDbItemForEachAction : Action
    {
        public override int Id
        {
            get
            {
                return 1025;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?TableName", "?ParentProperty", "?ParentId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Create DB Item for each";
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

            bool addParentRelation = false;
            string parentRelationColumn = "";
            int parentId = -1;
            if(vars.ContainsKey("ParentProperty") && vars.ContainsKey("ParentId")
                && table.columns.Exists(c => c.Name == (string)vars["ParentProperty"]))
            {
                addParentRelation = true;
                parentRelationColumn = (string)vars["ParentProperty"];
                parentId = (int)vars["ParentId"];
            }

            DBItem item = new DBItem();
            foreach (DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    item.createProperty(column.ColumnId, column.Name,
                        vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"));
                else if (vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"))
                {
                    if (column.type == "datetime")
                    {
                        try
                        {
                            item.createProperty(column.ColumnId, column.Name,
                                DateTime.Parse((string)vars[$"__Model.{table.tableName}.{column.Name}"]));
                        }
                        catch (FormatException)
                        {
                            // skip empty date instead of crashing
                        }
                    }
                    else
                        item.createProperty(column.ColumnId, column.Name,
                            vars[$"__Model.{table.tableName}.{column.Name}"]);
                }
            }
            if(addParentRelation)
            {
                item.createProperty(table.columns.Find(c=>c.Name == parentRelationColumn).ColumnId, parentRelationColumn, parentId);
            }
            table.Add(item);
            for (int panelIndex = 1; vars.ContainsKey($"panelCopy{panelIndex}Marker"); panelIndex++)
            {
                item = new DBItem();
                foreach (DBColumn column in table.columns)
                {
                    if (column.type == "bit")
                        item.createProperty(column.ColumnId, column.Name,
                            vars.ContainsKey($"__Model.panelCopy{panelIndex}.{table.tableName}.{column.Name}"));
                    else if (vars.ContainsKey($"__Model.panelCopy{panelIndex}.{table.tableName}.{column.Name}"))
                    {
                        if (column.type == "datetime")
                        {
                            try
                            {
                                item.createProperty(column.ColumnId, column.Name,
                                    DateTime.Parse((string)vars[$"__Model.{table.tableName}.{column.Name}"]));
                            }
                            catch (FormatException)
                            {
                                // skip empty date instead of crashing
                            }
                        }
                        else
                            item.createProperty(column.ColumnId, column.Name,
                                vars[$"__Model.panelCopy{panelIndex}.{table.tableName}.{column.Name}"]);
                    }
                }
                if (addParentRelation)
                {
                    item.createProperty(table.columns.Find(c => c.Name == parentRelationColumn).ColumnId, parentRelationColumn, parentId);
                }
                table.Add(item);
            }
            core.Entitron.Application.SaveChanges();
        }
    }
}
