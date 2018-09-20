using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class GenerateComparisonGraphAction : Action
    {

        public override int Id
        {
            get
            {
                return 3009;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "TableData", "LabelColumn", "ValueColumn", "StaticValueColumn" };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Generate comparison graph action";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
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
                StringBuilder sb = new StringBuilder();
                string staticValueCol = "Expected";
                
                double staticValue = vars["StaticValueColumn"].ToString().Contains(',') ?  Convert.ToDouble(vars["StaticValueColumn"].ToString().Replace(',','.')) : Convert.ToDouble(vars["StaticValueColumn"].ToString());
                string labelColumn = (string)vars["LabelColumn"];
                string valColumn = (string)vars["ValueColumn"];
                sb.AppendFormat("{0},{1},{2},{3}", labelColumn,staticValueCol,staticValueCol,valColumn);
                sb.Append("\\n");  //new line
                foreach (DBItem di in dbitems)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", di[labelColumn], staticValue.ToString().Replace(",","."), staticValue.ToString().Replace(",", "."), di[valColumn].ToString().Replace(",", "."));
                    sb.Append("\\n");  //new 
                }




                outputVars["Result"] = sb.ToString(); //retur comma seperated string  to result,which can be used by the graph later
            }
            else
            {
                outputVars["Result"] = "";
            }
        }
    }
}
