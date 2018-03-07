using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class FormatDatetime : Action
    {
        public override int Id => 187;

        public override string[] InputVar => new string[] { "Input", "?Format" };

        public override string Name => "Format DateTime";

        public override string[] OutputVar => new string[] { "Result","Error"};

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DateTime dateTime = (DateTime)vars["Input"];
            string format = vars.ContainsKey("Format") ? (string)vars["Format"] : "o";
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                outputVars["Result"] = DateTime.ParseExact(vars["Input"].ToString(), format, provider).ToString();
                outputVars["Error"] = false;
            }
            catch (FormatException)
            {
                outputVars["Result"] = null;
                outputVars["Error"] = true;
            }


        }
    }
}
