using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class MaxValueAction : Action
    {
        public override int Id => 1038;

        public override string[] InputVar => new string[] {"TableData", "Column", "MaxValue"};

        public override string Name => "Max value restriction";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var tableData = (List<DBItem>) vars["TableData"];
            var column = (string) vars["Column"];
            var maxValue = (int) vars["MaxValue"];

            if (tableData.Count == 0)
                return;

            List<DBItem> listItems = new List<DBItem>();
            int currentValue = 0;
            foreach (var row in tableData)
            {
                listItems.Add(row);
                currentValue += (int) row[column];

                if (currentValue >= maxValue)
                    break;
            }

            outputVars["Result"] = listItems;
        }
    }
}
