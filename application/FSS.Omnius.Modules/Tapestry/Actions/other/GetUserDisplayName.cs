using FSS.Omnius.Modules.CORE;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GetUserDisplayName : Action
    {
        public override int Id
        {
            get
            {
                return 501;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override string Name
        {
            get
            {
                return "Get user's displayName";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "DisplayName"
                };
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
            outputVars["DisplayName"] = core.User.DisplayName;
        }
    }
}
