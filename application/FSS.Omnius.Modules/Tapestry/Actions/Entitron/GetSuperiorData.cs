using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Table;
using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetSuperiorData : Action
    {
        public override int Id
        {
            get
            {
                return 1024;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "?Pernr" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get superior data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "SuperiorData", "NoSuperior"
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
            var tableUsers = core.Entitron.GetDynamicTable("Users");
            DBItem epkUser = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).FirstOrDefault();
            int userPernr;
            if(vars.ContainsKey("Pernr"))
                userPernr = (int)vars["Pernr"];
            else if (epkUser != null)
                userPernr = (int)epkUser["pernr"];
            else
                throw new Exception("Váš účet není propojen s tabulkou Users");

            DBView superiorView = core.Entitron.GetDynamicView("SuperiorMapping");
            DBItem superiorMapping = superiorView.Select().where(c => c.column("pernr").Equal(userPernr)).FirstOrDefault();
            DBItem result;
            DBView userView = core.Entitron.GetDynamicView("UserView");
            if (superiorMapping != null)
            {
                outputVars["NoSuperior"] = false;
                var superiorPernr = superiorMapping["superior_pernr"];
                var assistantView = core.Entitron.GetDynamicView("AssistantMapping");
                var assistantMapping = assistantView.Select().where(c => c.column("pernr").Equal(superiorPernr)).FirstOrDefault();
                if (assistantMapping != null)
                {
                    var assistentPernr = assistantMapping["assistant_pernr"];
                    result = userView.Select().where(c => c.column("pernr").Equal(assistentPernr)).FirstOrDefault();
                }
                else
                {
                    result = userView.Select().where(c => c.column("pernr").Equal(superiorPernr)).FirstOrDefault();
                }
            }
            else
            {
                result = userView.Select().where(c => c.column("pernr").Equal(userPernr)).FirstOrDefault();
                outputVars["NoSuperior"] = true;
            }
            result.createProperty(1000, "DisplayName", result["vorna"] + " " + result["nachn"]);
            outputVars["SuperiorData"] = result;
        }
    }
}
