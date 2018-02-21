using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetUserData : Action
    {
        public override int Id => 1023;

        public override string[] InputVar => new string[] { "?SearchInShared" };

        public override string Name => "Get user data";

        public override string[] OutputVar => new string[] { "UserData" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            DBConnection db = Modules.Entitron.Entitron.i;
            var user = core.User;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            DBItem result = db.Table("Users", searchInShared).Select()
                .Where(c => c.Column("ad_email").Equal(user.Email)).FirstOrDefault();

            result["RweId"] = result["id"];

            result["Id"] = user.Id;
            result["UserName"] = user.UserName;
            result["DisplayName"] = user.DisplayName;
            result["Email"] = user.Email;
            result["Job"] = user.Job;
            result["Company"] = user.Company;
            result["Address"] = user.Address;

            outputVars["UserData"] = result;
        }
    }
}
