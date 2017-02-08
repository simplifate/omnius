using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System.Globalization;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class UpdateDBItemWithoutFormAction : Action
    {
        public override int Id
        {
            get
            {
                return 1008;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?TableName", "?Id", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Update DBItem Without Form";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
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
            Modules.Entitron.Entitron ent = core.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            int itemId = vars.ContainsKey("Id")
                ? (int)vars["Id"]
                : (int)vars["__ModelId__"];
            DBTable table = ent.GetDynamicTable(tableName, searchInShared);
            if (table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem row = table.Select().where(c => c.column("Id").Equal(itemId)).First();
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {tableName}, Id: {itemId}, Akce: {Name} ({Id}))");

            foreach (DBColumn column in table.columns.Where(c => vars.ContainsKey($"__Model.{tableName}.{c.Name}")))
            {
                var inputValue = vars[$"__Model.{tableName}.{column.Name}"];
                if (column.type == "datetime" && inputValue is string)
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
            if (table.columns.Exists(c => c.Name == "id_user_change"))
            {
                row["id_user_change"] = core.User.Id;
                row["datetime_change"] = DateTime.Now;
            }
            table.Update(row, itemId);
            ent.Application.SaveChanges();
        }
    }
}
