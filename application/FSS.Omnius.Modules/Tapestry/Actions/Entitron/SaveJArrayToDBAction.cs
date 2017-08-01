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
  
        public class SaveJArrayToDBAction : Action
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
                    return "SaveJArrayToDBAction";
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
                    throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

                JArray jarray = (JArray)vars["JArray"];
                foreach (JObject jo in jarray) {
                DBItem item = new DBItem();
                    //iterate properties of jobject
                    foreach (JProperty prop in jo.Properties()) {
                    item.createProperty(0, prop.Name, jo.GetValue(prop.Name));
                    //now we have dbitem with allproperties and values, we would add the item do db
                    table.Add(item);
                    }
                core.Entitron.Application.SaveChanges();
            }


        }
    }
}
