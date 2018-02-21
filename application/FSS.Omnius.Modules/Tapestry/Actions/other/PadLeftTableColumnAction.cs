using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class PadLeftTableColumnAction : Action
    {
        public override int Id => 217;
        public override int? ReverseActionId => null;
        public override string[] InputVar => new string[] { "s$TableName", "s$ColumnName", "i$Length", "?s$PaddingChar", "?SearchInShared" };
        public override string Name => "Pad Left Table Column";
        public override string[] OutputVar => new string[0] { };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            DBTable table = db.Table((string)vars["TableName"], searchInShared);
            string colName = (string)vars["ColumnName"];
            char padding = vars.ContainsKey("PaddingChar") ? Convert.ToChar(vars["PaddingChar"]) : '0';

            var tableRowsList = table.Select().ToList();

            foreach(var tableRow in tableRowsList)
            {
                string columnContent = tableRow[colName].ToString();
                tableRow[colName] = columnContent.PadLeft((int)vars["Length"], padding);
                table.Update(tableRow,(int)tableRow["id"]);
            }

            db.SaveChanges();
        }
    }
}
    