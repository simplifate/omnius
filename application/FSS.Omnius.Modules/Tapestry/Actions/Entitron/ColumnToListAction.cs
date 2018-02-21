using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ColumnToListAction : Action
    {
        public override int Id => 198;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableRows", "ColumnName" };

        public override string Name => "Column to list";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (!vars.ContainsKey("TableRows"))
            {
                throw new Exception("Tapestry action Column to List expect TableRows parameter which was not supplied");
            }
            if (!vars.ContainsKey("ColumnName"))
            {
                throw new Exception("Tapestry action Column to List expect ColumnName parameter which was not supplied");
            }

            List<DBItem> Data = (List<DBItem>)vars["TableRows"];
            string columnName = (string)vars["ColumnName"];
            List<object> result = new List<object>();

            foreach (DBItem row in Data)
            {
                if (!row.HasProperty(columnName))
                {
                    throw new Exception($"Tapestry action Column to List can't found column {columnName} in supplied data");
                }
                result.Add(row[columnName]);
            }

            outputVars["Result"] = result;
        }
    }
}
