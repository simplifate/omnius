using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
  
    public class JArrayInsertIntoDB : Action
    {
        public override int Id => 5222;

        public override string[] InputVar => new string[] { "JArray","?TableName", "?SearchInShared" };

        public override string Name => "JArray: Insert into DB";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = COREobject.i.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                :(string)vars["__TableName__"];
            DBTable table = db.Table(tableName, searchInShared);

            if (table == null)
                throw new Exception($"Queried table not found! (Table: {tableName}, Action: {Name} ({Id}))");
            if (!vars.ContainsKey("JArray"))
                throw new Exception("JArray parameter not passed!");

            JArray jarray = (JArray)vars["JArray"];
                
            foreach (JObject jo in jarray)
            {
                Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);

                DBItem parsedRow = new DBItem(db, null);
                foreach (var parsedCol in parsedColumns)
                    parsedRow[parsedCol.Key] = parsedCol.Value;

                DBItem item = new DBItem(db, table);
                foreach (DBColumn col in table.Columns)
                {
                    if (col.Name == DBCommandSet.PrimaryKey)
                        continue;
                    string parsedColName = (col.Name == "ext_id") ? DBCommandSet.PrimaryKey : col.Name;
                    item[col.Name] = parsedRow[parsedColName];
                }

                table.Add(item);
            }
            db.SaveChanges();

            outputVars["Result"] = "Successful";
        }
    }
}
