using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ParseDatetime : Action
    {
        public override int Id
        {
            get
            {
                return 200;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Input" };
            }
        }

        public override string Name
        {
            get
            {
                return "Parse Datetime";
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
            string input = (string)vars["Input"];
            DateTime datetime;
            DateTime.TryParseExact(input,
                                new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-ddTHH:mm", "dd.MM.yyyy HH:mm", },
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime);

            outputVars["Result"] = datetime;
        }
    }
}
