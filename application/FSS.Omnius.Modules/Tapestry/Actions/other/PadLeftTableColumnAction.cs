using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class PadLeftTableColumnAction : Action
    {
        public override int Id
        {
            get
            {
                return 217;
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
                return new string[] { "TableName", "ColumnName", "Length", "?PaddingChar", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Pad Left Table Column";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0] { };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            DBTable table = core.Entitron.GetDynamicTable((string)vars["TableName"], searchInShared);
            string colName = (string)vars["ColumnName"];
            char padding = vars.ContainsKey("PaddingChar") ? Convert.ToChar(vars["PaddingChar"]) : '0';

            var tableRowsList = table.Select().ToList();

            foreach(var tableRow in tableRowsList)
            {
                string columnContent = tableRow[colName].ToString();
                tableRow[colName] = columnContent.PadLeft((int)vars["Length"], padding);
                table.Update(tableRow,(int)tableRow["id"]);
            }

            ent.Application.SaveChanges();
        }
    }
}
    