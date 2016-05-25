using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetUserData : Action
    {
        public override int Id
        {
            get
            {
                return 1023;
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
                return "Get user data";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "UserData"
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
            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";
            var result = new DBItem();
            var user = core.User;

            result.createProperty(0, "Id", user.Id);
            result.createProperty(1, "UserName", user.UserName);
            result.createProperty(2, "DisplayName", user.DisplayName);
            result.createProperty(3, "Email", user.Email);
            result.createProperty(4, "Job", user.Job);
            result.createProperty(5, "Company", user.Company);
            result.createProperty(6, "Address", user.Address);

            var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                        .where(c => c.column("ad_email").Equal(user.Email)).ToList();
            if (epkUserRowList.Count > 0)
            {
                var userRecord = epkUserRowList[0];
                result.createProperty(20, "kostl", userRecord["kostl"]);
                result.createProperty(21, "sapid1", userRecord["sapid1"]);
                result.createProperty(22, "sapid2", userRecord["sapid2"]);
                result.createProperty(23, "RweId", userRecord["id"]);
                result.createProperty(24, "pernr", userRecord["pernr"]);
            }
            outputVars["UserData"] = result;
        }
    }
}
