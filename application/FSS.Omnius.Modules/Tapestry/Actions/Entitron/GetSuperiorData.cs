using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
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
            var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
            int userPernr = 0;
            if(vars.ContainsKey("Pernr"))
            {
                userPernr = (int)vars["Pernr"];
            }
            else if (epkUserRowList.Count > 0)
            {
                userPernr = (int)epkUserRowList[0]["pernr"];
            }
            else
            {
                throw new Exception("Váš účet není propojen s tabulkou Users");
            }
            var superiorView = core.Entitron.GetDynamicView("SuperiorMapping");
            var superiorMappingList = superiorView.Select().where(c => c.column("pernr").Equal(userPernr)).ToList();
            DBItem result;
            if (superiorMappingList.Count > 0)
            {
                outputVars["NoSuperior"] = false;
                var superiorPernr = superiorMappingList[0]["superior_pernr"];
                var assistantView = core.Entitron.GetDynamicView("AssistantMapping");
                var assistantMappingList = assistantView.Select().where(c => c.column("pernr").Equal(superiorPernr)).ToList();
                if (assistantMappingList.Count > 0)
                {
                    var assistentPernr = assistantMappingList[0]["assistant_pernr"];
                    var assistentResultList = tableUsers.Select().where(c => c.column("pernr").Equal(assistentPernr)).ToList();
                    result = assistentResultList[0];
                }
                else
                {
                    var superiorResultList = tableUsers.Select().where(c => c.column("pernr").Equal(superiorPernr)).ToList();
                    result = superiorResultList[0];
                }
            }
            else
            {
                result = epkUserRowList[0];
                outputVars["NoSuperior"] = true;
            }
            result.createProperty(1000, "DisplayName", result["vorna"] + " " + result["nachn"]);
            outputVars["SuperiorData"] = result;
        }
    }
}
