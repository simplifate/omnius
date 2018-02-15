using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBConnection db = Modules.Entitron.Entitron.i;

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
                    if (column.Type == DbType.DateTime && vars[modelColumnName] is string)
                    {
                        DateTime parsedDateTime = new DateTime();
                        bool parseSuccessful = DateTime.TryParseExact((string)vars[modelColumnName],
                            new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-dd H:mm:ss", "yyyy-MM-dd" },
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                        if (parseSuccessful)
                            item[column.Name] = parsedDateTime;
                    }
                    else
                        item[column.Name] = vars[modelColumnName];
                }
            }
            DateTime timestamp = DateTime.Now;
            string timestampColumn = "";
            if (table.Columns.Any(c => c.Name == "ID_USER_VLOZIL"))
            {
                int userId = core.User.Id;
                item["ID_USER_VLOZIL"] = userId;
                item["ID_USER_EDITOVAL"] = userId;
                item["DATUM_VLOZENI"] = timestamp;
                item["DATUM_EDITACE"] = timestamp;
                timestampColumn = "DATUM_VLOZENI";
            }
            else if (table.Columns.Any(c => c.Name == "date"))
            {
                item["date"] = timestamp;
                timestampColumn = "date";
            }
            else if (table.Columns.Any(c => c.Name == "date_purchase"))
            {
                item["date_purchase"] = timestamp;
                timestampColumn = "date_purchase";
            }
            table.AddGetId(item);
            outputVars["AssignedId"] = item["id"];
        }
    }
}
