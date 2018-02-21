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
    public class UpdateDBItemAction : Action
    {
        public override int Id => 1007;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "?TableName", "?Id", "?SearchInShared" };

        public override string Name => "Update DB Item";

        public override string[] OutputVar => new string[0];
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
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

            foreach (DBColumn column in table.Columns)
            {
                string modelColumnName = $"__Model.{tableName}.{column.Name}";

                if (column.Type == DbType.Boolean)
                    row[column.Name] = vars.ContainsKey(modelColumnName);
                else if (vars.ContainsKey(modelColumnName))
                {
                    var inputValue = vars[modelColumnName];
                    if (column.Type == DbType.DateTime && inputValue is string)
                    {
                        DateTime parsedDateTime = new DateTime();
                        bool parseSuccessful = DateTime.TryParseExact((string)inputValue,
                        new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-dd H:mm:ss", "yyyy-MM-dd"},
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                        if (parseSuccessful)
                            row[column.Name] = parsedDateTime;
                        else
                            row[column.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[column.Name] = inputValue;
                    }
                }
            }

            if (table.Columns.Any(c => c.Name == "ID_USER_EDITOVAL"))
            {
                row["ID_USER_EDITOVAL"] = core.User.Id;
                row["DATUM_EDITACE"] = DateTime.Now;
            }

            table.Update(row, itemId);
            db.SaveChanges();
        }
    }
}
