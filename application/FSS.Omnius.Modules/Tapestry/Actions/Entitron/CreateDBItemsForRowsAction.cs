using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDBItemsForRows : Action
    {
        public override int Id
        {
            get
            {
                return 1037;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "InputData", "KeyMapping" };
            }
        }

        public override string Name
        {
            get
            {
                return "Create DB Items for for rows";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string tableName = (string)vars["TableName"];
            var inputData = (List<DBItem>)vars["InputData"];
            string keyMappingString = (string)vars["KeyMapping"];
            var keyMappingDictionary = new Dictionary<string, string>();

            foreach(var segment in keyMappingString.Split(','))
            {
                var pair = segment.Split(':');
                if(pair.Length == 2)
                {
                    keyMappingDictionary.Add(pair[0], pair[1]);
                }
            }

            var table = core.Entitron.GetDynamicTable(tableName);
            DateTime timestamp = DateTime.Now;
            foreach (var inputRow in inputData)
            {
                var newRowItem = new DBItem();
                foreach (DBColumn column in table.columns)
                {
                    if(keyMappingDictionary.ContainsKey(column.Name))
                    {
                        string dictionaryValue = keyMappingDictionary[column.Name];
                        if (dictionaryValue.Substring(0, 2) == "$$")
                            newRowItem.createProperty(column.ColumnId, column.Name, inputRow[dictionaryValue.Substring(2)]);
                        else
                            newRowItem.createProperty(column.ColumnId, column.Name, KeyValueString.ParseValue(dictionaryValue, vars));
                    }
                }
                if (table.columns.Exists(c => c.Name == "id_user_insert"))
                {
                    int userId = core.User.Id;
                    newRowItem.createProperty(table.columns.Find(c => c.Name == "id_user_insert").ColumnId, "id_user_insert", userId);
                    newRowItem.createProperty(table.columns.Find(c => c.Name == "id_user_change").ColumnId, "id_user_change", userId);
                    newRowItem.createProperty(table.columns.Find(c => c.Name == "datetime_insert").ColumnId, "datetime_insert", timestamp);
                    newRowItem.createProperty(table.columns.Find(c => c.Name == "datetime_change").ColumnId, "datetime_change", timestamp);
                }
                table.Add(newRowItem);
            }
            core.Entitron.Application.SaveChanges();
        }
    }
}
