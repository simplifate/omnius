using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class InitToDatetime : Action
    {
        public override int Id
        {
            get
            {
                return 950;
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
                return new string[] { "Int" };
            }
        }

        public override string Name
        {
            get
            {
                return "Int To Datetime";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            int timestamp = Convert.ToInt32(vars["Int"]);

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            outputVars["Result"] = dateTime.AddSeconds(timestamp).ToLocalTime();
        }
    }
}
