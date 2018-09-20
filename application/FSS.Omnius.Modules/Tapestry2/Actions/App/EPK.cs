using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using System;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public class App_EPK : ActionManager
    {
        [Action(1023, "Get user data", "UserData")]
        public static DBItem GetUserData(COREobject core, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            var user = core.User;

            DBItem result = db.Table("Users", SearchInShared).Select()
                .Where(c => c.Column("ad_email").Equal(user.Email)).FirstOrDefault();

            result["RweId"] = result["id"];

            result["Id"] = user.Id;
            result["UserName"] = user.UserName;
            result["DisplayName"] = user.DisplayName;
            result["Email"] = user.Email;
            result["Job"] = user.Job;
            result["Company"] = user.Company;
            result["Address"] = user.Address;

            return result;
        }

        [Action(1024, "Get superior data", "SuperiorData", "NoSuperior")]
        public static (DBItem, bool) GetSuperiorData(COREobject core, bool SearchInShared = false, int? Pernr = null)
        {
            DBConnection db = core.Entitron;
            bool resultSupperior;
            DBItem resultItem;

            var tableUsers = db.Table("Users", SearchInShared);
            DBItem epkUser = tableUsers.Select().Where(c => c.Column("ad_email").Equal(core.User.Email)).FirstOrDefault();
            if (epkUser == null)
                throw new Exception("Váš účet není propojen s tabulkou Users");

            int userPernr = Pernr ?? (int)epkUser["pernr"];

            Tabloid superiorView = db.Tabloid("SuperiorMapping", SearchInShared);
            DBItem superiorMapping = superiorView.Select().Where(c => c.Column("pernr").Equal(userPernr)).FirstOrDefault();
            Tabloid userView = db.Tabloid("UserView", SearchInShared);
            if (superiorMapping != null)
            {
                resultSupperior = false;
                var superiorPernr = superiorMapping["superior_pernr"];
                var assistantView = db.Tabloid("AssistantMapping", SearchInShared);
                var assistantMapping = assistantView.Select().Where(c => c.Column("pernr").Equal(superiorPernr)).FirstOrDefault();
                if (assistantMapping != null)
                {
                    var assistentPernr = assistantMapping["assistant_pernr"];
                    resultItem = userView.Select().Where(c => c.Column("pernr").Equal(assistentPernr)).FirstOrDefault();
                }
                else
                {
                    resultItem = userView.Select().Where(c => c.Column("pernr").Equal(superiorPernr)).FirstOrDefault();
                }
            }
            else
            {
                resultItem = userView.Select().Where(c => c.Column("pernr").Equal(userPernr)).FirstOrDefault();
                resultSupperior = true;
            }
            resultItem["DisplayName"] = resultItem["vorna"] + " " + resultItem["nachn"];

            return (resultItem, resultSupperior);
        }

        [Action(1000001, "Fill Order", "PurchaseDate")]
        public static string FillOrder(COREobject core)
        {
            // CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            // var sap = core.Entitron.GetDynamicTable("Users");
            // string username = core.User.UserName;
            // int i = username.IndexOf('\\') >= 0 ? username.IndexOf('\\') : 0;
            // var thisSap = sap.Select().where(c => c.column("ad_id").Equal(username.Substring(i))).First();

            // var plan = core.Entitron.GetDynamicTable("Plans");
            // var thisSapPlan = plan.Select().where(c => c.column("objid").Equal(thisSap["plans"])).First();

            // outputVars["year"] = DateTime.Now.Year;
            // outputVars["client_sapid2"] = thisSap["sapid2"];
            // outputVars["client_function"] = thisSapPlan["stext"];
            return DateTime.Now.ToString("dd.MM.y");
        }
    }
}
