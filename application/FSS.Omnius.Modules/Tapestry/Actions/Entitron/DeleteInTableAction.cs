using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class DeleteInTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1938;
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
                return new string[] { "TableName", "?SearchInShared", "CondColumn[index]", "CondValue[index]", "?CondOperator[index]" };
            }
        }

        public override string Name
        {
            get
            {
                return "Delete in table";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"];
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string orderBy = vars.ContainsKey("OrderBy") ? (string)vars["OrderBy"] : null;
            bool isDescending = vars.ContainsKey("Descending") ? (bool)vars["Descending"] : false;

            ListJson<DBItem> columns = db.ExecuteRead(db.CommandSet.LIST_column(db, tableName), new DBTable(db) { Name = tableName });

            //
            var delete = db.Table(tableName).Delete();
            int CondCount = vars.Keys.Where(k => k.StartsWith("CondColumn[") && k.EndsWith("]")).Count();

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                string condOperator = vars.ContainsKey($"CondOperator[{i}]") ? (string)vars[$"CondOperator[{i}]"] : "Equal";
                string condColumn = (string)vars[$"CondColumn[{i}]"];
                object condValue = vars[$"CondValue[{i}]"];

                DBItem column = columns.Single(c => (string)c["name"] == condColumn);
                object value = condOperator != "IsIn"
                    ? DataType.ConvertTo(DataType.FromDBName((string)column["typeName"], db.Type), condValue)
                    : column["typeName"];

                switch (condOperator)
                {
                    case "Less":
                        delete.Where(c => c.Column(condColumn).Less(value));
                        break;
                    case "LessOrEqual":
                        delete.Where(c => c.Column(condColumn).LessOrEqual(value));
                        break;
                    case "Greater":
                        delete.Where(c => c.Column(condColumn).Greater(value));
                        break;
                    case "GreaterOrEqual":
                        delete.Where(c => c.Column(condColumn).GreaterOrEqual(value));
                        break;
                    case "Equal":
                        delete.Where(c => c.Column(condColumn).Equal(value));
                        break;
                    case "IsIn":
                        // string, multiple values
                        if ((condValue is string) && ((string)condValue).Contains(","))
                            delete.Where(c => c.Column(condColumn).In((condValue as string).Split(',')));
                        // Enumerable
                        else
                            delete.Where(c => c.Column(condColumn).In((IEnumerable<object>)condValue));
                        break;
                    default: // ==
                        delete.Where(c => c.Column(condColumn).Equal(value));
                        break;
                }
            }

            // order
            delete.Run();
        }
    }
}
