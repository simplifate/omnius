using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class FilterTableByColumn : Action
    {
        public override int Id
        {
            get
            {
                return 1052;
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
                return new string[] { "TableData", "ColumnName" };
            }
        }

        public override string Name
        {
            get
            {
                return "Filter Table By Column";
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
            List<DBItem> tableData = (List<DBItem>)vars["TableData"];
            string columnName = (string)vars["ColumnName"];
            if (!tableData[0].HasProperty(columnName))
                throw new Exception("Column name not found!");

            List<DBItem> outputTable = new List<DBItem>();
            var uniques = new HashSet<object>();

            foreach(DBItem tableRow in tableData)
            {   
                if (!uniques.Contains(tableRow[columnName]))
                {
                    uniques.Add(tableRow[columnName]);
                    outputTable.Add(tableRow);
                }
            }

            outputVars["Result"] = outputTable;
        }
    }
}
