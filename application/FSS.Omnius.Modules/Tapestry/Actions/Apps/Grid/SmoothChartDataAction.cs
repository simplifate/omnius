using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class SmoothChartDataAction : Action
    {
        public override int Id
        {
            get
            {
                return 213;
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
                return new string[] { "TableData", "s$Column", "i$Range" };
            }
        }

        public override string Name
        {
            get
            {
                return "Smooth Chart Data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "TableData" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<DBItem> tableData = (List<DBItem>)vars["TableData"];
            string column = (string)vars["Column"];
            int range = (int)vars["Range"];

            for(int i = tableData.Count - 1; i >= 0; i--)
            {
                List<double> avgVals = new List<double>();
                for (int j = 0; j < range; j++)
                {
                    if (i - j < 0)
                        break;
                    avgVals.Add((double)tableData[i-j][column]);
                }
                tableData[i][column] = avgVals.Average();
            }
            outputVars["TableData"] = tableData;
        }
    }
}
