using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDbItemForEachAction : Action
    {
        public override int Id => 1025;

        public override string[] InputVar => new string[] { "?TableName", "?ParentProperty", "?ParentId", "?SearchInShared" };

        public override string Name => "Create DB Item for each";

        public override string[] OutputVar => new string[] { };

        public override int? ReverseActionId => 1010;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = db.Table(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            bool addParentRelation = false;
            string parentRelationColumn = "";
            int parentId = -1;
            if (vars.ContainsKey("ParentProperty") && vars.ContainsKey("ParentId")
                && table.Columns.Any(c => c.Name == (string)vars["ParentProperty"]))
            {
                addParentRelation = true;
                parentRelationColumn = (string)vars["ParentProperty"];
                parentId = (int)vars["ParentId"];
            }

            DBItem item = new DBItem(db, table);
            foreach (DBColumn column in table.Columns)
            {
                if (column.Type == DbType.Boolean)
                    item[column.Name] = vars.ContainsKey($"__Model.{table.Name}.{column.Name}");
                else if (vars.ContainsKey($"__Model.{table.Name}.{column.Name}"))
                {
                    if (column.Type == DbType.DateTime)
                    {
                        try
                        {
                            item[column.Name] = DateTime.ParseExact((string)vars[$"__Model.{table.Name}.{column.Name}"], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            // skip empty date instead of crashing
                        }
                    }
                    else
                        item[column.Name] = vars[$"__Model.{table.Name}.{column.Name}"];
                }
            }
            if (addParentRelation)
            {
                item[parentRelationColumn] = parentId;
            }
            table.Add(item);
            for (int panelIndex = 1; vars.ContainsKey($"panelCopy{panelIndex}Marker"); panelIndex++)
            {
                item = new DBItem(db, table);
                foreach (DBColumn column in table.Columns)
                {
                    if (column.Type == DbType.Boolean)
                        item[column.Name] = vars.ContainsKey($"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}");
                    else if (vars.ContainsKey($"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}"))
                    {
                        if (column.Type == DbType.DateTime)
                        {
                            try
                            {
                                item[column.Name] = DateTime.ParseExact((string)vars[$"__Model.{table.Name}.{column.Name}"], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (FormatException)
                            {
                                // skip empty date instead of crashing
                            }
                        }
                        else
                            item[column.Name] = vars[$"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}"];
                    }
                }
                if (addParentRelation)
                {
                    item[parentRelationColumn] = parentId;
                }
                table.Add(item);
            }
            db.SaveChanges();
        }
    }
}
