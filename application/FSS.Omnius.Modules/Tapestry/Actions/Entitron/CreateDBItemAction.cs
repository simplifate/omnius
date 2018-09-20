using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CreateDbItemAction : Action
    {
        public override int Id => 1004;

        public override string[] InputVar => new string[] { "?TableName", "?ReturnAssignedId", "?SearchInShared" };

        public override string Name => "Create DB Item";

        public override string[] OutputVar => new string[] { "AssignedId" };

        public override int? ReverseActionId => 1010;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = db.Table(tableName, searchInShared);

            DBItem item = new DBItem(db, table);
            foreach (DBColumn column in table.Columns)
            {
                string modelColumnName = $"__Model.{table.Name}.{column.Name}";

                if (column.Type == DbType.Boolean)
                    item[column.Name] = vars.ContainsKey(modelColumnName);
                else if (vars.ContainsKey(modelColumnName))
                {
                    if (column.Type == System.Data.DbType.DateTime)
                    {
                        if (vars[modelColumnName] is DateTime)
                            item[column.Name] = vars[modelColumnName];
                        else
                        {
                            DateTime parsedDateTime = new DateTime();
                            bool parseSuccessful = DateTime.TryParseExact((string)vars[modelColumnName],
                                new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-dd H:mm:ss", "dd.MM.yyyy HH:mm", "yyyy-MM-dd" },
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                            if (parseSuccessful)
                                item[column.Name] = parsedDateTime;
                        }
                    }
                    else if (vars[modelColumnName] is JValue)
                    {
                        if (column.Type == System.Data.DbType.Int32)
                            item[column.Name] = ((JValue)vars[modelColumnName]).ToObject<int>();
                        if (column.Type == System.Data.DbType.Double)
                            item[column.Name] = ((JValue)vars[modelColumnName]).ToObject<double>();
                    }
                    else
                        item[column.Name] = vars[modelColumnName];
                }
            }
            DateTime timestamp = DateTime.Now;
            if (table.Columns.Any(c => c.Name == "ID_USER_VLOZIL"))
            {
                int userId = core.User.Id;
                item["ID_USER_VLOZIL"] = userId;
                item["ID_USER_EDITOVAL"] = userId;
                item["DATUM_VLOZENI"] = timestamp;
                item["DATUM_EDITACE"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date"))
            {
                item["date"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date_purchase"))
            {
                item["date_purchase"] = timestamp;
            }
            table.AddGetId(item);
            outputVars["AssignedId"] = item["id"];
        }
    }
}
