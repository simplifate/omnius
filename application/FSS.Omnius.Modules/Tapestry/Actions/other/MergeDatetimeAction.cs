using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class MergeDatetime : Action
    {
        public override int Id
        {
            get
            {
                return 194;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Date", "Time" };
            }
        }

        public override string Name
        {
            get
            {
                return "Merge Date and Time";
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
            DateTime date = (DateTime)vars["Date"];
            DateTime time = (DateTime)vars["Time"];

            DateTime result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

            outputVars["Result"] = result;
        }
    }
}
