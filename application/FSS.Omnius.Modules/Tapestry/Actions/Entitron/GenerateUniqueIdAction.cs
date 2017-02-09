using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GenerateUniqueIdAction : Action
    {
        public override int Id
        {
            get
            {
                return 1048;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableName", "ColumnName", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Generate unique ID";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
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

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            string tableName = (string)vars["TableName"];
            string columnName = (string)vars["ColumnName"];

            var table = core.Entitron.GetDynamicTable(tableName, searchInShared);
            var results = table.Select().ToList();
            int prevoiusId = results.Count > 0 ? results.Select(c => (int)c[columnName]).Max() : 0;
            outputVars["Result"] = prevoiusId + 1;
        }
    }
}
