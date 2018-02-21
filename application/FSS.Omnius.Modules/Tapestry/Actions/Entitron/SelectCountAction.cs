using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class SelectCountAction : Action
    {
        public override int Id => 1040;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableData" };

        public override string Name => "Select count";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            int tableDataCount = tableData.Count;

            outputVars["Result"] = tableDataCount;
        }
    }
}
