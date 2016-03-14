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
    public class ReadDBItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 1022;
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
                return new string[] { "TableName", "Id" };
            }
        }

        public override string Name
        {
            get
            {
                return "Select Item (by Id)";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            outputVars["Data"] = core.Entitron.GetDynamicItem((string)vars["TableName"], (int)vars["Id"]);
            if(outputVars["Data"] == null)
            {
                string msg = string.Format("Položka nebyla nalezena (Tabulka: {0}, Id: {1}, Akce: {2} ({3}))", vars["TableName"], vars["Id"], Name, Id);

                WatchtowerLogger logger = WatchtowerLogger.Instance;
                logger.LogEvent(
                    msg,
                    core.User.Id,
                    LogEventType.Tapestry,
                    LogLevel.Error,
                    false,
                    core.Entitron.AppId
                );

                throw new Exception(msg);
            }
        }
    }
}
