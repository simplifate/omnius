using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetCellFromRowAction : Action
    {
        public override int Id => 1027;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "RowData", "ColumnName" };

        public override string Name => "Get cell from row";

        public override string[] OutputVar => new string[] { "CellValue" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBItem rowData = vars["RowData"] is List<DBItem> ? ((List<DBItem>)vars["RowData"])[0] : (DBItem)vars["RowData"];
            outputVars["CellValue"] = rowData[(string)vars["ColumnName"]];
        }
    }
}
