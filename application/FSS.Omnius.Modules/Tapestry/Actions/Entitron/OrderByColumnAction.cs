using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    class OrderByColumnAction : Action
    {
        public override int Id
        {
            get { return 1039; }
        }

        public override string[] InputVar
        {
            get { return new string[] { "TableData", "Column1", "?Column2" }; }
        }

        public override string Name
        {
            get { return "Order by column"; }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get { return null; }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars,
            Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>)vars["TableData"];
            var column1 = (string)vars["Column1"];
            var column2 = (string)vars["Column2"];

            if (tableData.Count == 0)
                return;

            List<DBItem> sortedTableData;
            if (vars.ContainsKey("Column2"))
                sortedTableData = tableData.OrderBy(x => x[column1]).ThenBy(x => x[column2]).ToList();
            else
                sortedTableData = tableData.OrderBy(x => x[column1]).ToList();
            
            outputVars["Result"] = sortedTableData;
        }
    }
}
