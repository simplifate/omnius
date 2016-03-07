using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    public class ShowMessageAction : Action
    {
        public override int Id
        {
            get
            {
                return 182;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Message" };
            }
        }

        public override string Name
        {
            get
            {
                return "Show message";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars)
        {
            (outputVars["__Messages__"] as Message).Info.Add((string)vars["Message"]);
        }
    }
}
