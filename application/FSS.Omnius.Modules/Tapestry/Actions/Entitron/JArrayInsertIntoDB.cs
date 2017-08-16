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
  
    public class JArrayInsertIntoDB : Action
    {
        public override int Id
        {
            get
            {
                return 5222;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "JArray","?TableName", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "JArray: Insert into DB";
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
                :(string)vars["__TableName__"];
            DBTable table = core.Entitron.GetDynamicTable(tableName, searchInShared);

            if (table == null)
                throw new Exception($"Queried table not found! (Table: {tableName}, Action: {Name} ({Id}))");
            if (!vars.ContainsKey("JArray"))
                throw new Exception("JArray parameter not passed!");

            JArray jarray = (JArray)vars["JArray"];
                
            foreach (JObject jo in jarray)
            {
                Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                TapestryUtils.ParseJObject(jo, parsedColumns);

                DBItem parsedRow = new DBItem();
                int colId = 0;
                foreach (var parsedCol in parsedColumns)
                    parsedRow.createProperty(colId++, parsedCol.Key, parsedCol.Value);

                DBItem item = new DBItem();
                colId = 0;
                foreach (DBColumn col in table.columns)
                {
                    if (col.Name == "id")
                        continue;
                    string parsedColName = (col.Name == "ext_id") ? "id" : col.Name;
                    item.createProperty(colId++, col.Name, parsedRow[parsedColName]);
                }

                table.Add(item);

            }
            core.Entitron.Application.SaveChanges();
            outputVars["Result"] = "Successful";
        }
    }
}
