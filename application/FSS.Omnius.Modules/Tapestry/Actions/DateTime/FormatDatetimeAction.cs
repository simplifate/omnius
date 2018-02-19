using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class FormatDatetime : Action
    {
        public override int Id
        {
            get
            {
                return 187;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Input", "?Format" };
            }
        }

        public override string Name
        {
            get
            {
                return "Format DateTime";
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
            DateTime dateTime = (DateTime)vars["Input"];
            string format = vars.ContainsKey("Format") ? (string)vars["Format"] : "o";

            outputVars["Result"] = dateTime.ToLocalTime().ToString(format);
        }
    }
}
