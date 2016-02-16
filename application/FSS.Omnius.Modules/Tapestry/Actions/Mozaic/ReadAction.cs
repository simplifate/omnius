using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class ReadAction : Action
    {
        public override int Id
        {
            get
            {
                return 2001;
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
                return "Read";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Data" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBTable table = core.Entitron.GetDynamicTable((string)vars["TableName"]);
            var select = table.Select();
            Conditions condition = new Conditions(select);
            Condition_concat outCondition = null;

            outCondition = condition.column("Id").Equal(vars["Id"]);
            condition = outCondition.and();

            vars["Data"] = select.where(i => outCondition).ToList();
            if(((List<DBItem>)vars["Data"]).Count() == 0)
            {
                string message = String.Format("Položka nebyla nalezena (Tabulka: {0}, Id: {1})", vars["TableName"], vars["Id"]);

                WatchtowerLogger logger = WatchtowerLogger.Instance;
                logger.LogEvent(
                    message,
                    core.User.Id,
                    LogEventType.NormalUserAction,
                    LogLevel.Error,
                    false,
                    core.Entitron.AppId
                );

                throw new Exception(message);
            }
        }
    }
}
