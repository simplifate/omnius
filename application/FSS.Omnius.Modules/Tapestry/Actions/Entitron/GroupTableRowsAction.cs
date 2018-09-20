using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    /// <summary>

    /// </summary>
    [EntitronRepository]
    public class GroupTableRowsAction : Action
    {
        public override int Id => 1050;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableData", "GroupingColumn", "?DateTimeSensitivity" };

        public override string[] OutputVar => new string[] { "Result" };

        public override string Name => "Group table rows";
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            List<DBItem> tableData = (List<DBItem>)vars["TableData"];
            string groupingColumn = (string)vars["GroupingColumn"];
            int dtSensitivity = vars.ContainsKey("DateTimeSensitivity") ? Convert.ToInt32((vars["DateTimeSensitivity"])) : 3;
            List<DBItem> result = new List<DBItem>();
            List<DBItem> sortedTableData = tableData.OrderBy(c => c[groupingColumn]).ToList();
            
            //
            if (tableData.Count > 0)
            {
                // isDateTime - use DateTimeSensitivity
                bool isDateTime = sortedTableData[0][groupingColumn] is DateTime;

                // Sort tableData at first
                for (int i = 0; i < (sortedTableData.Count - 1); i++)
                {
                    var currentRow = sortedTableData[i];
                    var nextRow = sortedTableData[i+1];
                    
                    // groups
                    if ((!isDateTime && currentRow[groupingColumn].ToString() == nextRow[groupingColumn].ToString()) 
                        || (isDateTime && System.Math.Abs((Convert.ToDateTime(currentRow[groupingColumn]) - Convert.ToDateTime(nextRow[groupingColumn])).TotalMinutes) <= dtSensitivity))
                    {
                        // sum column - add to nextColumn
                        // skip id, group column, non-numeric
                        foreach (string columnName in currentRow.getColumnNames().Where(c => c.ToLower() != DBCommandSet.PrimaryKey.ToLower() && c != groupingColumn && (currentRow[c] is int || currentRow[c] is double)))
                        {
                            double current = currentRow[columnName] != DBNull.Value ? Convert.ToDouble(currentRow[columnName]) : 0;
                            current += nextRow[columnName] != DBNull.Value ? Convert.ToDouble(nextRow[columnName]) : 0;
                            nextRow[columnName] = current;
                        }
                    }

                    // different item - stop grouping
                    else
                        result.Add(currentRow);
                }

                // add last item
                result.Add(sortedTableData.Last());
            }

            outputVars["Result"] = result;
        }
    }
}