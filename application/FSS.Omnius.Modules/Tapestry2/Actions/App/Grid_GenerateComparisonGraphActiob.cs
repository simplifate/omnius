using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3009, "Grid: Generate comparison graph action", "Result")]
        public static string GenerateComparisonGraph(COREobject core, List<DBItem> TableData, string LabelColumn, string ValueColumn, double StaticValueColumn)
        {
            if (TableData.Count <= 0)
                return "";
            
            StringBuilder sb = new StringBuilder();
            string staticValueCol = "Expected";
                
            sb.AppendFormat("{0},{1},{2},{3}", LabelColumn, staticValueCol, staticValueCol, ValueColumn);
            sb.Append("\\n");  //new line
            foreach (DBItem di in TableData)
            {
                sb.AppendFormat("{0},{1},{2},{3}", di[LabelColumn], StaticValueColumn.ToString().Replace(",","."), StaticValueColumn.ToString().Replace(",", "."), di[ValueColumn].ToString().Replace(",", "."));
                sb.Append("\\n");  //new 
            }
            
            return sb.ToString();
        }
    }
}
