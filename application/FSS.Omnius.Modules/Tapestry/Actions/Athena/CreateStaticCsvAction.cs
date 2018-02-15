using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class CreateStaticCSV : Action
    {
        public override int Id => 5501;

        public override string[] InputVar => new string[] { "column1", "column2", "label1", "value1", "label2", "value2" };

        public override string Name => "CreateStaticCSV";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

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