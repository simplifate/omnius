using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ModifyDatetime : Action
    {
        public override int Id
        {
            get
            {
                return 188;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Input", "?AddMinutes", "?AddDays" };
            }
        }

        public override string Name
        {
            get
            {
                return "Modify DateTime";
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
            if (vars.ContainsKey("AddMinutes"))
                dateTime = dateTime.AddMinutes((int)vars["AddMinutes"]);

            if (vars.ContainsKey("AddDays"))
                dateTime = dateTime.AddDays((int)vars["AddDays"]);

            // TODO: Other modifications: add hours, days, etc. + add timespan

            outputVars["Result"] = dateTime;
        }
    }
}
