using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class LogAction : Action
    {
        public override int Id => 2002;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "Message", "?Level" };

        public override string Name => "Log message";

        public override string[] OutputVar => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            string msg = vars.ContainsKey("Message") ? (string)vars["Message"] : string.Empty;
            OmniusLogLevel level = vars.ContainsKey("Level") ? (OmniusLogLevel)vars["Level"] : OmniusLogLevel.Info;

            OmniusLog.Log(msg, level, OmniusLogSource.User, core.Application, core.User);
        }
    }
}
