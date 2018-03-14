using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]

    public class JArrayUpdateDB : Action
    {
        public override int Id => 5223;

        public override string[] InputVar => new string[] { "JArray", "?UniqueCol", "?TableName", "?SearchInShared" };

        public override string Name => "JArray: Update DB";

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
                    throw new Exception($"Queried table not found (Tabulka: {tableName}, Akce: {Name} ({Id}))");
                var listDbItem = table.Select().ToList();
                //check if table has column (IsDeleted),if no , the result is null
                var columnIsDeletedExist = table.Columns.SingleOrDefault(c => c.Name == "IsDeleted");
           

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
                if (!table.Columns.Any(c => c.Name == uniqueCol))
                    throw new Exception($"Table column named '{uniqueCol}' not found!");
                foreach (JObject jo in jarray)
                {
                    if (columnIsDeletedExist != null)
                    {
                        //if theres column IsDeleted, check if the entity is in rising, if not, set isDeleted to true.
                        for(int i =0;i < listDbItem.Count;i++)
                        {
                            if(!jarray.Any(j=> j["id"].ToString() != listDbItem[i]["ext_id"].ToString()))
                            {
                                DBItem foundItem = listDbItem[i];
                                foundItem["IsDeleted"] = true;
                                table.Update(foundItem, (int)foundItem["ext_id"]);
                            }
                        }
                    }
                    Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                    TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);
                    DBItem parsedRow = new DBItem(db, table, parsedColumns);

                    DBItem updatedRow = table.Select().Where(c => c.Column(uniqueCol).Equal(parsedRow[uniqueExtCol])).FirstOrDefault();

                    if (updatedRow != null) //update
                    {
                        foreach (var col in parsedRow.getColumnNames())
                        {
                            if (updatedRow.getColumnNames().Contains(col) && col != DBCommandSet.PrimaryKey && col != uniqueCol)
                            {
                                updatedRow[col] = parsedRow[col];
                            }
                        }
                        table.Update(updatedRow, (int)updatedRow[DBCommandSet.PrimaryKey]);
                    }
                    else // insert row if it does not exist 
                    {
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
