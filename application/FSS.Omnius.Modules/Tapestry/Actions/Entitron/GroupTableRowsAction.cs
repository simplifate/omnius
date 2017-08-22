using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    /// <summary>

    /// </summary>
    [EntitronRepository]
    public class GroupTableRowsAction : Action
    {
        public override int Id
        {
            get
            {
                return 1050;
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
                return new string[] { "TableData", "GroupingColumn", "?DateTimeSensitivity" };
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get
            {
                return "Group table rows";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<DBItem> tableData = (List<DBItem>)vars["TableData"];
            string groupingColumn = (string)vars["GroupingColumn"];
            List<DBItem> result = new List<DBItem>();
            int dtSensitivity = vars.ContainsKey("DateTimeSensitivity") ? Convert.ToInt32((vars["DateTimeSensitivity"])) : 3;
            // If table contains more than 0 rows
            if (tableData.Count > 0)
            {
                // Sort tableData at first
                List<DBItem> sortedTableData = tableData.OrderBy(c => c[groupingColumn]).ToList();
                DBItem element = sortedTableData[0];
                for (int i = 0; i < sortedTableData.Count; i++)
                {
                    var isLastElement = false;
                    var currentRow = sortedTableData[i];
                    bool isDateTime = currentRow[groupingColumn] is DateTime; ;
                    if ((!isDateTime && currentRow[groupingColumn].ToString() == element[groupingColumn].ToString()) || (isDateTime && (Convert.ToDateTime(currentRow[groupingColumn]) - Convert.ToDateTime(element[groupingColumn])).TotalMinutes <= dtSensitivity))
                    {
                        foreach (string columnName in element.getColumnNames())
                        {
                            if ((element[columnName] is int || element[columnName] is double) && columnName.ToUpper() != "id".ToUpper())
                            {
                                double current = currentRow[columnName] != DBNull.Value ? Convert.ToDouble(currentRow[columnName]) : 0;
                                current += element[columnName] != DBNull.Value ? Convert.ToDouble(element[columnName]) : 0;
                                element[columnName] = current;
                                if (i == sortedTableData.Count - 1)
                                {
                                    element[columnName] = current;
                                    isLastElement = true;
                                }
                            }
                        }
                        if(isLastElement)
                            result.Add(element);
                    }
                    else
                    {
                        result.Add(element);

                        element = currentRow;
                        if (i == sortedTableData.Count - 1)
                            result.Add(currentRow);
                    }
                }
            }

            outputVars["Result"] = result;
        }
    }
}