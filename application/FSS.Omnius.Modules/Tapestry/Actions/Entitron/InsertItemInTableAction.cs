using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class InsertItemInTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 9957;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?TableName", "?Item"};
            }
        }

        public override string Name
        {
            get
            {
                return "Create DB Item";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "AssignedId" };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return 1010;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            DBTable table = core.Entitron.GetDynamicTable(tableName);

            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem item = (DBItem)vars["Item"];
            foreach (DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    item.createProperty(column.ColumnId, column.Name, vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"));
                else if (vars.ContainsKey($"__Model.{table.tableName}.{column.Name}"))
                {
                    if (column.type == "datetime" && vars[$"__Model.{table.tableName}.{column.Name}"] is string)
                    {
                        DateTime parsedDateTime = new DateTime();
                        bool parseSuccessful = DateTime.TryParseExact((string)vars[$"__Model.{table.tableName}.{column.Name}"],
                            new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-dd H:mm:ss", "yyyy-MM-dd"},
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                        if (parseSuccessful)
                            item.createProperty(column.ColumnId, column.Name, parsedDateTime);
                    }
                    else
                        item.createProperty(column.ColumnId, column.Name, vars[$"__Model.{table.tableName}.{column.Name}"]);
                }
            }
            DateTime timestamp = DateTime.Now;
            string timestampColumn = "";
            if (table.columns.Exists(c => c.Name == "ID_USER_VLOZIL"))
            {
                int userId = core.User.Id;
                item.createProperty(table.columns.Find(c => c.Name == "ID_USER_VLOZIL").ColumnId, "ID_USER_VLOZIL", userId);
                item.createProperty(table.columns.Find(c => c.Name == "ID_USER_EDITOVAL").ColumnId, "ID_USER_EDITOVAL", userId);
                item.createProperty(table.columns.Find(c => c.Name == "DATUM_VLOZENI").ColumnId, "DATUM_VLOZENI", timestamp);
                item.createProperty(table.columns.Find(c => c.Name == "DATUM_EDITACE").ColumnId, "DATUM_EDITACE", timestamp);
                timestampColumn = "DATUM_VLOZENI";
            }
            else if (table.columns.Exists(c => c.Name == "date"))
            {
                item.createProperty(table.columns.Find(c => c.Name == "date").ColumnId, "date", timestamp);
                timestampColumn = "date";
            }
            else if (table.columns.Exists(c => c.Name == "date_purchase"))
            {
                item.createProperty(table.columns.Find(c => c.Name == "date_purchase").ColumnId, "date_purchase", timestamp);
                timestampColumn = "date_purchase";
            }
            table.Add(item);
            core.Entitron.Application.SaveChanges();

            if (timestampColumn != "")
            {
                var searchResults = table.Select().where(c => c.column(timestampColumn).Equal(timestamp)).ToList();
                if(searchResults.Count > 0)
                    outputVars["AssignedId"] = searchResults[0]["id"];
            }
        }
    }
}
