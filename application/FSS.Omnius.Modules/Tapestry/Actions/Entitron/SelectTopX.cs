using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class SelectTopXAction : Action
    {
        public override int Id
        {
            get
            {
                return 10036;
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
                return new string[] { "TableData", "SortingColumn", "NumOfItems" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select top X";
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
            String pocet = vars["NumOfItems"].ToString();
            int x = Convert.ToInt32(pocet);
            var tableData = (List<DBItem>)vars["TableData"];
            string sortingColumn = (string)vars["SortingColumn"];
            var result = tableData.OrderByDescending(c => c[sortingColumn]).Take(x);
            outputVars["Result"] = result.ToList();
        }
    }
}
