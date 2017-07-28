using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Tapestry.Actions.other;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class CreateStaticCSV : Action
    {
        public override int Id
        {
            get
            {
                return 5501;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "column1", "column2", "label1", "value1", "label2", "value2" };
            }
        }
        public override string Name
        {
            get
            {
                return "CreateStaticCSV";
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
            String column1 = vars["column1"].ToString();
            String column2 = vars["column2"].ToString();
            String label1 = vars["label1"].ToString();
            String value1 = vars["value1"].ToString();
            String label2 = vars["label2"].ToString();
            String value2 = vars["value2"].ToString();
            string s = $"{column1},{column2}\\n{label1},{value1}\\n{label2},{value2}";
            outputVars["Result"] = s;
        }
    }
}