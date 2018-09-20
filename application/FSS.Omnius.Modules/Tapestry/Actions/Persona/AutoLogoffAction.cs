using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Persona
{
    [Obsolete]
    public class AutoLogoffAction : Action
    {
        public override int Id => 4105;

        public override string[] InputVar => new string[] { };

        public override string Name => "User auto logoff";

        public override string[] OutputVar => new string[] { };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            //Modules.Persona.Persona.AutoLogOff();
        }
    }
}