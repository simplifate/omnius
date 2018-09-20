using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class InsertItemInTableAction : Action
    {
        public override int Id => 9957;

        public override string[] InputVar => new string[] { "?TableName", "?Item" };

        public override string Name => "Insert DB Item";

        public override string[] OutputVar => new string[] { "AssignedId" };

        public override int? ReverseActionId => 1010;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = db.Table(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem item = (DBItem)vars["Item"];
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
            outputVars["AssignedId"] = table.AddGetId(item);
        }
    }
}
