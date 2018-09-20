using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Athena : ActionManager
    {
        [Action(5501, "Create Static CSV", "Result")]
        public static string CreateStaticCSV(COREobject core, string[] column, string[] label, string[] value)
        {
            return $"{column[1]},{column[2]}\\n{label[1]},{value[1]}\\n{label[2]},{value[2]}";
        }

        [Action(5505, "Calibrate Chart Data", "Result")]
        public static List<DBItem> CalibrateChartData(COREobject core, List<DBItem> TableData, string ColumnName)
        {
            if (TableData.Count <= 0)
                return new List<DBItem>();

            var ListColValue = TableData.Select(d => d[ColumnName]);
            double minVal = Convert.ToDouble(ListColValue.Min());
            for (int i = 0; i < TableData.Count(); i++)
            {
                TableData[i][ColumnName] = Convert.ToDouble(TableData[i][ColumnName]) - minVal;
            }
            return TableData.OrderBy(x => x["ColumnName"]).ToList();
        }
    }
}
