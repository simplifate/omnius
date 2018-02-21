using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDBItemsForRows : Action
    {
        public override int Id => 1037;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "InputData", "KeyMapping", "?SearchInShared" };

        public override string Name => "Create DB Items for for rows";

        public override string[] OutputVar => new string[0];

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBConnection db = Modules.Entitron.Entitron.i;

            string tableName = (string)vars["TableName"];
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            var inputData = (List<DBItem>)vars["InputData"];
            string keyMappingString = (string)vars["KeyMapping"];
            var keyMappingDictionary = new Dictionary<string, string>();

            foreach (var segment in keyMappingString.Split(','))
            {
                var pair = segment.Split(':');
                if (pair.Length == 2)
                {
                    keyMappingDictionary.Add(pair[0], pair[1]);
                }
            }

            DBTable table = db.Table(tableName, searchInShared);
            DateTime timestamp = DateTime.Now;
            foreach (var inputRow in inputData)
            {
                var newRowItem = new DBItem(db, table);
                foreach (DBColumn column in table.Columns)
                {
                    if (keyMappingDictionary.ContainsKey(column.Name))
                    {
                        string dictionaryValue = keyMappingDictionary[column.Name];
                        if (dictionaryValue.Substring(0, 2) == "$$")
                            newRowItem[column.Name] = inputRow[dictionaryValue.Substring(2)];
                        else
                            newRowItem[column.Name] = KeyValueString.ParseValue(dictionaryValue, vars);
                    }
                }
                if (table.Columns.Any(c => c.Name == "id_user_insert"))
                {
                    int userId = core.User.Id;
                    newRowItem["id_user_insert"] = userId;
                    newRowItem["id_user_change"] = userId;
                    newRowItem["datetime_insert"] = timestamp;
                    newRowItem["datetime_change"] = timestamp;
                }
                table.Add(newRowItem);
            }
            db.SaveChanges();
        }
    }
}
