using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    /// <summary>
    /// Přijme table data a vypočítá průměry každých X (defaultně 100) záznamů 
    /// </summary>
    class TableDataAveragesAction : Action
    {
        public override int Id
        {
            get
            {
                return 21399;
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
                return new string[] { "TableData", "DataColName", "?i$Range" };
            }
        }

        public override string Name
        {
            get
            {
                return "TableData Group Average";
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
            int range = (vars.ContainsKey("Range")) ? (int)vars["Range"] : 100;
            List<DBItem> data = (List<DBItem>)vars["TableData"];
            string dataColName = (string)vars["DataColName"];
            int dataCount = data.Count;
            int divisor = dataCount / range;
            List<DBItem> group = new List<DBItem>();
            List<DBItem> result = new List<DBItem>();
            for(int i = 0; i < dataCount; i++)
            {
                group.Add(data[i]);
                
                if(i % divisor == 0)
                {
                    //calculate average
                    int sum = 0;
                    foreach(var dbi in group)
                    {
                        sum += Convert.ToInt32(dbi[dataColName].ToString());
                    }
                    data[i][dataColName] = sum / divisor;
                    result.Add(data[i]);
                    group.Clear();
                }
            }

            outputVars["TableData"] = result;
        }
    }
}
