using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetModelUrlAction : Action
    {
        public override int Id
        {
            get
            {
                return 1031;
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
                return new string[] { "BlockName", "Id" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get Model URL";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Entitron.AppName;
            string blockName = (string)vars["BlockName"];
            int modelId = (int)vars["Id"];
            outputVars["Result"] = $"{hostname}/{appName}/{blockName}?modelId={modelId}";
        }
    }
}
