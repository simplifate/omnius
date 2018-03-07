using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using System.Globalization;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ParseDateTimeAction : Action
    {
        public override int Id => 18712;

        public override string[] InputVar => new string[] { "Input", "?Format" };

        public override string Name => "Parse DateTime";

        public override string[] OutputVar => new string[] { "Result","HasError" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            try
            {
                string format = vars.ContainsKey("Format") ? (string)vars["Format"] : "";
                CultureInfo provider = CultureInfo.InvariantCulture;
                if(format != "")
                {
                    outputVars["Result"] = DateTime.ParseExact(vars["Input"].ToString(), format, provider);
                    outputVars["HasError"] = false;
                }
                else
                {
                    outputVars["Result"] = DateTime.Parse(vars["Input"].ToString());
                    outputVars["HasError"] = false;
                }

            }
            catch (FormatException)
            {
                outputVars["Result"] = null;
                outputVars["HasError"] = true;
            }

        }
    }
}
