using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SaveListAction : Action
    {
        public override int Id => 1106;
        public override string[] InputVar => new string[] { "tableName", "list" };
        public override string[] OutputVar => new string[] { };
        public override int? ReverseActionId => null;
        public override string Name => "Save list";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string tableName = (string)vars["tableName"];
            IEnumerable<DBItem> list = (IEnumerable<DBItem>)vars["list"];
            DBTable targetTable = core.Entitron.GetDynamicTable(tableName);

            foreach (DBItem item in list)
            {
                targetTable.Add(item);
            }
            core.Entitron.Application.SaveChanges();
        }
    }
}
