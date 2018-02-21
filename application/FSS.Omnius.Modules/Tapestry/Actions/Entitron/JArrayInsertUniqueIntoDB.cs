using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]

    public class JArrayInsertUniqueIntoDB : Action
    {
        public override int Id => 5224;

        public override string[] InputVar => new string[] { "JArray", "?UniqueCol", "?TableName", "?SearchInShared" };

        public override string Name => "JArray: Insert Unique into DB";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JArray jarray = (JArray)vars["JArray"];
            if (jarray.HasValues)
            {
                DBConnection db = Modules.Entitron.Entitron.i;

                bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

                string tableName = vars.ContainsKey("TableName")
                    ? (string)vars["TableName"]
                    : (string)vars["__TableName__"];
                DBTable table = db.Table(tableName, searchInShared);
                if (table == null)
                    throw new Exception($"Queried table not found! (Table: {tableName}, Action: {Name} ({Id}))");

                string uniqueCol;
                string uniqueExtCol; //basicly foreign key
                if (vars.ContainsKey("UniqueCol"))
                {
                    uniqueCol = (string)vars["UniqueCol"];
                    uniqueExtCol = uniqueCol;
                }
                else
                {
                    uniqueCol = "ext_id";
                    uniqueExtCol = DBCommandSet.PrimaryKey;
                }
                if (!table.Columns.Any(c => c.Name == uniqueExtCol))
                    throw new Exception($"Table column named '{uniqueExtCol}' not found!");

                HashSet<string> tableUniques = new HashSet<string>(table.Select(uniqueCol).ToList().Select(x => x[uniqueCol].ToString()).ToList());

                HashSet<DBItem> parsedItems = new HashSet<DBItem>();
                foreach (JObject jo in jarray)
                {
                    Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                    TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);
                    
                    parsedItems.Add(new DBItem(db, table, parsedColumns));
                }
                HashSet<string> parsedExtIds = new HashSet<string>(); //ids of items from input jarray
                foreach (var item in parsedItems)
                    parsedExtIds.Add(item[uniqueExtCol].ToString());
                HashSet<string> newUniqueIDs = new HashSet<string>(parsedExtIds.Except(tableUniques));
                parsedItems.RemoveWhere(x => !newUniqueIDs.Contains(x[uniqueExtCol]));

                foreach (DBItem parsedItem in parsedItems)
                {
                    DBItem item = new DBItem(db, table);
                    foreach (DBColumn col in table.Columns)
                    {
                        if (col.Name == DBCommandSet.PrimaryKey)
                            continue;
                        string parsedColName = (col.Name == "ext_id") ? DBCommandSet.PrimaryKey : col.Name;
                        if (parsedItem[parsedColName] != null)//
                            item[col.Name] = parsedItem[parsedColName];
                    }

                    table.Add(item);
                }
                db.SaveChanges();
                
                outputVars["Result"] = true;
            }
            else
            {
                Watchtower.OmniusLog.Log($"{Name}: Input JArray has no values! Action aborted", Watchtower.OmniusLogLevel.Warning);
                outputVars["Result"] = false;
            }
        }
    }
}



