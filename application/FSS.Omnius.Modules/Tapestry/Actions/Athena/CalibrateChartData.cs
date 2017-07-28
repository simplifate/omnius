using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Tapestry.Actions.other;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class CalibrateChartData : Action
    {
        public override int Id
        {
            get
            {
                return 5505;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableData","ColumnName"};
            }
        }
        public override string Name
        {
            get
            {
                return "Calibrate Chart Data";
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<DBItem> dbitems = (List<DBItem>)vars["TableData"];
            if (dbitems.Count > 0)
            { //IF theres is more than 0 element in the List!!!
                var ListColValue = dbitems.Select(d => d[(string)vars["ColumnName"]]);
                double minVal = Convert.ToDouble(ListColValue.Min());
                for (int i = 0; i < dbitems.Count(); i++) {
                    dbitems[i][(string)vars["ColumnName"]] = Convert.ToDouble(dbitems[i][(string)vars["ColumnName"]]) - minVal;
                }
                outputVars["Result"] = dbitems.OrderBy(x => x["ColumnName"]).ToList();
            }
            else
                outputVars["Result"] = new List<DBItem>();
        }
    }
}