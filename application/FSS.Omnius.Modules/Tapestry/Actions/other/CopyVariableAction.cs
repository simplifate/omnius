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
    public class CopyVariableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1278;
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
                return new string[] { "From" };
            }
        }

        public override string Name
        {
            get
            {
                return "Copy variable";
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
            if (vars["From"] is string)
            {
                outputVars["Result"] = ((string)vars["From"]).Replace("\\n", "\n");
            }
            else
            {
                outputVars["Result"] = vars["From"];
            }
        }
    }
}
