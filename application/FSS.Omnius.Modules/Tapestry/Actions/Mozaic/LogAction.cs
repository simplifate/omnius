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
                return "Log";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            WatchtowerLogger logger = WatchtowerLogger.Instance;

            string message = vars.ContainsKey("Message") ? (string)vars["Message"] : string.Empty;
            int level = vars.ContainsKey("Level") ? (int)vars["Level"] : (int)LogLevel.Info;

            if(string.IsNullOrEmpty(message))
            {
                logger.LogEvent(
                        string.Format("Zpráva nebyla předána (Akce: {0} ({1}))", Name, Id),
                        core.User.Id,
                        LogEventType.Tapestry,
                        LogLevel.Error,
                        false,
                        core.Entitron.AppId
                    );
                throw new Exception("Zpráva k zalogování nebyla předána");
            }

            logger.LogEvent(
                    message,
                    core.User.Id,
                    LogEventType.NormalUserAction,
                    (LogLevel)level,
                    false,
                    core.Entitron.AppId
                );
        }
    }
}
