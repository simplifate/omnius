using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectLastItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1524;
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
                return new string[] { "TableName", "SortingColumn" , "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select Last Item";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = COREobject.i.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;
            string columnName = (string)vars["SortingColumn"];
            string tableName = vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"];
            
            outputVars["Result"] = db.Select(tableName, searchInShared).Order(AscDesc.Desc, columnName).FirstOrDefault();
        }
    }
}
