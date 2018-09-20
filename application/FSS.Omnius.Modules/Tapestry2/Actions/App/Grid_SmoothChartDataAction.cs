using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(213, "Smooth Chart Data", "TableData")]
        public static List<DBItem> SmoothChartData(COREobject core, List<DBItem> TableData, string Column, int Range)
        {
            for(int i = TableData.Count - 1; i >= 0; i--)
            {
                List<double> avgVals = new List<double>();
                for (int j = 0; j < Range; j++)
                {
                    if (i - j < 0)
                        break;
                    avgVals.Add((double)TableData[i-j][Column]);
                }
                TableData[i][Column] = avgVals.Average();
            }
            return TableData;
        }
    }
}
