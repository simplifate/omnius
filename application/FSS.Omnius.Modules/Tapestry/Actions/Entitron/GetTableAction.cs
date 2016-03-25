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
    class GetTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1003;
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
                return new string[] { "TableName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get Table";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data", "columnNames" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBTable table = core.Entitron.GetDynamicTable(vars.ContainsKey("TableName") ? (string)vars["TableName"] : (string)vars["__TableName__"]);

            outputVars["columnNames"] = table.columns.Select(c => c.Name);
            outputVars["Data"] = table.Select().ToList();
        }
    }
}
