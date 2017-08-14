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

    public class JArrayUpdateDB : Action
    {
        public override int Id
        {
            get
            {
                return 5223;
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
                return "JArray: Update DB";
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

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = core.Entitron.GetDynamicTable(tableName, searchInShared);
            if (table == null)
                throw new Exception($"Queried table not found (Tabulka: {tableName}, Akce: {Name} ({Id}))");

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
            if (!table.columns.Exists(c => c.Name == uniqueCol))
                throw new Exception($"Table column named '{uniqueCol}' not found!");      

            JArray jarray = (JArray)vars["JArray"];
            foreach (JObject jo in jarray)
            {
                DBItem updatedRow = table.Select().where(c => c.column(uniqueCol).Equal(jo.GetValue(uniqueExtCol).ToObject<object>())).FirstOrDefault();
                if (updatedRow != null)
                {
                    foreach (JProperty prop in jo.Properties())
                    {
                        if (prop.Name != "id" && prop.Name != uniqueCol)
                            updatedRow[prop.Name] = prop.Value.ToObject<object>();
                    }
                    table.Update(updatedRow, (int)updatedRow["id"]);
                }
                else // insert row if it does not exist
                {
                    DBItem item = new DBItem();
                    int colId = 0;
                    foreach (JProperty prop in jo.Properties())
                    {
                        string property = (prop.Name == "id") ? "ext_id" : prop.Name;
                        item.createProperty(colId++, property, jo.GetValue(prop.Name).ToObject<object>());
                    }
                    table.Add(item);
                }        
            }
            core.Entitron.Application.SaveChanges();
            outputVars["Result"] = "Successful";
        }
    }
}
