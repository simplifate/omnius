using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectTopAction : Action
    {
        public override int Id => 1036;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableData", "SortingColumn" };

        public override string Name => "Select top";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            string sortingColumn = (string)vars["SortingColumn"];
            var result = tableData.OrderByDescending(c => c[sortingColumn]).First();
            outputVars["Result"] = result;
        }
    }
}
