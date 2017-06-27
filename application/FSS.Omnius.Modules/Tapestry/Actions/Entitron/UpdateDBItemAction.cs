using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class UpdateDBItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1007;
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
                return new string[] { "?TableName", "?Id", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Update DB Item";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;
            DBEntities e = new DBEntities();

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = vars.ContainsKey("TableName")
                ? (string)vars["TableName"]
                : (string)vars["__TableName__"];
            int itemId = vars.ContainsKey("Id")
                ? (vars["Id"] is int ? (int)vars["Id"] : Convert.ToInt32(vars["Id"]))
                : (int)vars["__ModelId__"];
            DBTable table = ent.GetDynamicTable(tableName, searchInShared);
            if(table == null)
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");

            DBItem row = table.Select().where(c => c.column("Id").Equal(itemId)).First();
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {tableName}, Id: {itemId}, Akce: {Name} ({Id}))");

            foreach (DBColumn column in table.columns)
            {
                if (column.type == "bit")
                    row[column.Name] = vars.ContainsKey($"__Model.{tableName}.{column.Name}");
                else if (vars.ContainsKey($"__Model.{tableName}.{column.Name}"))
                {
                    var inputValue = vars[$"__Model.{tableName}.{column.Name}"];
                    if (column.type == "datetime" && inputValue is string)
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

            if (table.columns.Exists(c => c.Name == "ID_USER_EDITOVAL"))
            {
                row["ID_USER_EDITOVAL"] = core.User.Id;
                row["DATUM_EDITACE"] = DateTime.Now;
            }

            table.Update(row, itemId);
            ent.Application.SaveChanges();
        }
    }
}
