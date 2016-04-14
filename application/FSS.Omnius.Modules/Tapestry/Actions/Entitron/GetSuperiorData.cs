using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
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
                return new string[0];
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
                    "SuperiorData"
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
            if (epkUserRowList.Count > 0)
            {
                int superiorId = (int)epkUserRowList[0]["h_pernr"];
                var epkSuperiorRowList = tableUsers.Select()
                        .where(c => c.column("pernr").Equal(superiorId)).ToList();
                DBItem result;
                if (epkSuperiorRowList.Count > 0)
                    result = epkSuperiorRowList[0];
                else
                    result = epkUserRowList[0];
                result.createProperty(1000, "DisplayName", result["vorna"] + " " + result["nachn"]);
                outputVars["SuperiorData"] = result;
            }
        }
    }
}
