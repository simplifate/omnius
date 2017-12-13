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
using System.Globalization;
namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>

    /// </summary>
    [EntitronRepository]
    public class TableColumnToCSV : Action
    {
        public override int Id
        {
            get
            {
                return 30104;
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
                return new string[] { "TableData", "LabelColumn", "ValueColumn1", "?ValueColumn2", "?ValueColumn3", "?ValueColumn4" };
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
                return "Table Column To CSV";
            }
        }
        private string checkNull(object Input)
        {
            if (Input == DBNull.Value || Input == null)
                return "";
            else
                return Input.ToString();
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            List<DBItem> dbitems = (List<DBItem>)vars["TableData"];

            if (dbitems.Count > 0)
            { //IF theres is more than 0 element in the List!!!
                StringBuilder sb = new StringBuilder();
                string labelColumn = (string)vars["LabelColumn"];
                string valColumn1 = (string)vars["ValueColumn1"];
                if (vars.ContainsKey("ValueColumn2") == false && vars.ContainsKey("ValueColumn3") == false && vars.ContainsKey("ValueColumn4") == false)
                { //if user set only labelCol and valueCOl1
                    sb.AppendFormat("{0},{1}", labelColumn, valColumn1);
                    sb.Append("\\n");  //new line
                    foreach (DBItem di in dbitems)
                    {
                        sb.AppendFormat("{0},{1}", checkNull(di[labelColumn]).Replace(",", "."), checkNull(di[valColumn1]).Replace(",", "."));
                        sb.Append("\\n");  //new lin4
                    }
                }

                if (vars.ContainsKey("ValueColumn2") && vars.ContainsKey("ValueColumn3") == false && vars.ContainsKey("ValueColumn4") == false)
                { //if there is valuecol1 and valCOl2
                    string valColumn2 = (string)vars["ValueColumn2"];
                    sb.AppendFormat("{0},{1},{2}", labelColumn, valColumn1, valColumn2);
                    sb.Append("\\n");  //new line
                    foreach (DBItem di in dbitems)
                    {
                        sb.AppendFormat("{0},{1},{2}", checkNull(di[labelColumn]).Replace(",", "."), checkNull(di[valColumn1]).Replace(",", "."), checkNull(di[valColumn2]).Replace(",", "."));
                        sb.Append("\\n");  //new line
                    }
                }
                if (vars.ContainsKey("ValueColumn2") && vars.ContainsKey("ValueColumn3") && vars.ContainsKey("ValueColumn4") == false)
                { //if there is valuecol1 and valCOl2
                    string valColumn2 = (string)vars["ValueColumn2"];
                    string valColumn3 = (string)vars["ValueColumn3"];
                    sb.AppendFormat("{0},{1},{2},{3}", labelColumn, valColumn1, valColumn2, valColumn3);
                    sb.Append("\\n");  //new line
                    foreach (DBItem di in dbitems)
                    {
                        sb.AppendFormat("{0},{1},{2},{3}", checkNull(di[labelColumn]).Replace(",", "."), checkNull(di[valColumn1]).Replace(",", "."), checkNull(di[valColumn2]).Replace(",", "."), checkNull(di[valColumn3]).Replace(",", "."));
                        sb.Append("\\n");  //new line
                    }
                }

                if (vars.ContainsKey("ValueColumn2") && vars.ContainsKey("ValueColumn3") && vars.ContainsKey("ValueColumn4"))
                {
                    string valColumn2 = (string)vars["ValueColumn2"];
                    string valColumn3 = (string)vars["ValueColumn3"];
                    string valColumn4 = (string)vars["ValueColumn4"];
                    sb.AppendFormat("{0},{1},{2},{3},{4}", labelColumn, valColumn1, valColumn2, valColumn3, valColumn4);
                    sb.Append("\\n");  //new line
                    foreach (DBItem di in dbitems)
                    {
                        sb.AppendFormat("{0},{1},{2},{3},{4}", di[labelColumn].ToString().Replace(",", "."), di[valColumn1].ToString().Replace(",", "."), di[valColumn2].ToString().Replace(",", "."), di[valColumn3].ToString().Replace(",", "."), di[valColumn4].ToString().Replace(",", "."));
                        sb.Append("\\n");  //new line
                    }
                }

                outputVars["Result"] = sb.ToString(); //retur comma seperated string  to result,which can be used by the graph later
            }
            else
                outputVars["Result"] = "";
        }
    }
}