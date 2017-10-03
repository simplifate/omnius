using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]

    public class JArrayInsertUniqueIntoDB : Action
    {
        public override int Id
        {
            get
            {
                return 5224;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "JArray", "?UniqueCol", "?TableName", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "JArray: Insert Unique into DB";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JArray jarray = (JArray)vars["JArray"];
            if (jarray.HasValues)
            {
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];

                bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

                string tableName = vars.ContainsKey("TableName")
                    ? (string)vars["TableName"]
                    : (string)vars["__TableName__"];
                DBTable table = core.Entitron.GetDynamicTable(tableName, searchInShared);
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
                    uniqueExtCol = "id";
                }
                if (!table.columns.Exists(c => c.Name == uniqueExtCol))
                    throw new Exception($"Table column named '{uniqueExtCol}' not found!");

                HashSet<string> tableUniques = new HashSet<string>(table.Select(uniqueCol).ToList().Select(x => x[uniqueCol].ToString()).ToList());

                HashSet<DBItem> parsedItems = new HashSet<DBItem>();
                foreach (JObject jo in jarray)
                {
                    Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                    TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);

                    DBItem parsedRow = new DBItem();
                    int colId = 0;
                    foreach (var parsedCol in parsedColumns)
                        parsedRow.createProperty(colId++, parsedCol.Key, parsedCol.Value);
                    parsedItems.Add(parsedRow);
                }
                HashSet<string> parsedExtIds = new HashSet<string>(); //ids of items from input jarray
                foreach (var item in parsedItems)
                    parsedExtIds.Add(item[uniqueExtCol].ToString());
                HashSet<string> newUniqueIDs = new HashSet<string>(parsedExtIds.Except(tableUniques));
                parsedItems.RemoveWhere(x => !newUniqueIDs.Contains(x[uniqueExtCol]));

                foreach (DBItem parsedItem in parsedItems)
                {
                    DBItem item = new DBItem();
                    int colId = 0;
                    foreach (DBColumn col in table.columns)
                    {
                        if (col.Name == "id")
                            continue;
                        string parsedColName = (col.Name == "ext_id") ? "id" : col.Name;
                        if (parsedItem[parsedColName] != null)//
                            item.createProperty(colId++, col.Name, parsedItem[parsedColName]);
                    }

                    table.Add(item);

                }

                core.Entitron.Application.SaveChanges();
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



