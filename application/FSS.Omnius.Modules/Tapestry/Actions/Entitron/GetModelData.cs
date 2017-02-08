using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetModelData : Action
    {
        public override int Id
        {
            get
            {
                return 1002;
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
                return new string[] { "ColumnId", "?SearchInShared" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get Model Data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            DBItem model = (vars["__CORE__"] as CORE.CORE).Entitron.GetDynamicItem((string)vars["__TableName__"], (int)vars["__ModelId__"], searchInShared);
            if (vars.ContainsKey("ColumnId"))
                outputVars["Data"] = model[(int)vars["ColumnId"]];
            else
                outputVars["Data"] = model;
        }
    }
}
