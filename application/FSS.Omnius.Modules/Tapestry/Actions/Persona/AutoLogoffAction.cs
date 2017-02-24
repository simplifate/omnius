using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Persona
{
    public class AutoLogoffAction : Action
    {
        public override int Id
        {
            get
            {
                return 4105;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override string Name
        {
            get
            {
                return "User auto logoff";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];

            core.Persona.AutoLogOff();
        }
    }
}