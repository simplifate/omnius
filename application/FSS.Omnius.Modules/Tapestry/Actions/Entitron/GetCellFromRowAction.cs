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
    public class GetCellFromRowAction : Action
    {
        public override int Id
        {
            get
            {
                return 1027;
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
                return new string[] { "RowData", "ColumnName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get cell from row";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "CellValue" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";
            outputVars["CellValue"] = ((DBItem)vars["RowData"])[(string)vars["ColumnName"]];
        }
    }
}
