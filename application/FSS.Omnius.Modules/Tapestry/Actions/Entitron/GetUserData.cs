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
            var user = core.User;

            DBItem result = core.Entitron.GetDynamicTable("Users").Select()
                                        .where(c => c.column("ad_email").Equal(user.Email)).FirstOrDefault();

            result.createProperty(1010, "RweId", result["id"]);

            result.createProperty(1000, "Id", user.Id);
            result.createProperty(1001, "UserName", user.UserName);
            result.createProperty(1002, "DisplayName", user.DisplayName);
            result.createProperty(1003, "Email", user.Email);
            result.createProperty(1004, "Job", user.Job);
            result.createProperty(1005, "Company", user.Company);
            result.createProperty(1006, "Address", user.Address);
            
            outputVars["UserData"] = result;
        }
    }
}
