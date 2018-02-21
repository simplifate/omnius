using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Data;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class UpdateDBItemWithoutFormAction : Action
    {
        public override int Id => 1008;

        public override string[] InputVar => new string[] { "?TableName", "?Id", "?SearchInShared" };

        public override string Name => "Update DBItem Without Form";

        public override string[] OutputVar => new string[] { };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            int itemId = vars.ContainsKey("Id")
                ? (vars["Id"] is int ? (int)vars["Id"] : Convert.ToInt32(vars["Id"]))
                : (int)vars["__ModelId__"];
            DBTable table = db.Table(tableName, searchInShared);

            DBItem row = table.SelectById(itemId);
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {tableName}, Id: {itemId}, Akce: {Name} ({Id}))");

            foreach (DBColumn column in table.Columns.Where(c => vars.ContainsKey($"__Model.{tableName}.{c.Name}")))
            {
                var inputValue = vars[$"__Model.{tableName}.{column.Name}"];
                if (column.Type == DbType.DateTime && inputValue is string)
                {
                    DateTime parsedDateTime = new DateTime();
                    bool parseSuccessful = DateTime.TryParseExact((string)inputValue,
                        new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                    if (parseSuccessful)
                        row[column.Name] = parsedDateTime;
                }
                else
                {
                    row[column.Name] = inputValue;
                }
            }
            if (table.Columns.Any(c => c.Name == "id_user_change"))
            {
                row["id_user_change"] = core.User.Id;
                row["datetime_change"] = DateTime.Now;
            }
            table.Update(row, itemId);
            db.SaveChanges();
        }
    }
}
