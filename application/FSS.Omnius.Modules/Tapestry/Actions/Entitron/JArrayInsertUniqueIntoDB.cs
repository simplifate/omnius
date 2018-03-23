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
                if (vars.ContainsKey("UniqueCol"))
                    uniqueCol = (string)vars["UniqueCol"];
                else
                    uniqueCol = "ext_id";
                
                if (!table.Columns.Any(c => c.Name == uniqueCol))
                    throw new Exception($"Table column named '{uniqueCol}' not found!");

                List<DBItem> currentDBItems = table.Select(uniqueCol).ToList();
                List<DBItem> newItemsFromJArray = new List<DBItem>();

                //iterate jarray and insert data to Omnius table,if theres unique column defined and a record with duplicate data,ignore it
                foreach (JObject j in jarray) {
                    var duplicatedItem = currentDBItems.SingleOrDefault(c => c[uniqueCol].ToString() == j["id"].ToString());
                    if (duplicatedItem != null)
                        continue;
                    DBItem item = new DBItem(db);
                    //get all properties of jObject entity
                    foreach(var prop in j.Properties())
                    {
                        if(prop.Name == "id")
                           item[uniqueCol] = ((JValue)j.GetValue(prop.Name)).Value;
                        else
                        item[prop.Name] = ((JValue)j.GetValue(prop.Name)).Value;
                    }
                    newItemsFromJArray.Add(item);
                }

                
                foreach(var item in newItemsFromJArray)
                {
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



