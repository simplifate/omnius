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
                return new string[] { "JArray", "UniqueCol", "?TableName", "?SearchInShared" };
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

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = core.Entitron.GetDynamicTable(tableName, searchInShared);
            if (table == null)
                throw new Exception($"Queried table not found! (Table: {tableName}, Action: {Name} ({Id}))");

            string uniqueCol = (string)vars["UniqueCol"];
            if (!table.columns.Exists(c => c.Name == uniqueCol))
                throw new Exception($"Table column named '{uniqueCol}' not found!");

            JArray jarray = (JArray)vars["JArray"];
            foreach (JObject jo in jarray)
            {
                //Skip if an item already exists in the table
                if (table.Select().ToList().Any(c => (string)c[uniqueCol] == jo.GetValue(uniqueCol).ToString()))
                    continue;
                DBItem insertedRow = new DBItem();
                int colId = 0;
                foreach (JProperty prop in jo.Properties())
                {
                    if (prop.Name != "id")
                        insertedRow.createProperty(colId++, prop.Name, jo.GetValue(prop.Name).ToObject<object>());
                }
                table.Add(insertedRow);
            }
            core.Entitron.Application.SaveChanges();

        }
    }
}



