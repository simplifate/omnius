using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class LogAction : Action
    {
        public override int Id
        {
            get
            {
                return 2002;
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
                return new string[] { "Message", "?Level" };
            }
        }

        public override string Name
        {
            get
            {
                return "Log message";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            string msg = vars.ContainsKey("Message") ? (string)vars["Message"] : string.Empty;
            OmniusLogLevel level = vars.ContainsKey("Level") ? (OmniusLogLevel)vars["Level"] : OmniusLogLevel.Info;

            OmniusLog.Log(msg, level, OmniusLogSource.User, core.Entitron.Application, core.User);
        }
    }
}
