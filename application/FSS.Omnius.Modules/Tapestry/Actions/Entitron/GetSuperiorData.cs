using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class GetSuperiorData : Action
    {
        public override int Id => 1024;

        public override string[] InputVar => new string[] { "?Pernr", "?SearchInShared" };

        public override string Name => "Get superior data";

        public override string[] OutputVar => new string[] { "SuperiorData", "NoSuperior" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;

            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            var tableUsers = db.Table("Users", searchInShared);
            DBItem epkUser = tableUsers.Select().Where(c => c.Column("ad_email").Equal(core.User.Email)).FirstOrDefault();
            int userPernr;
            if(vars.ContainsKey("Pernr"))
                userPernr = (int)vars["Pernr"];
            else if (epkUser != null)
                userPernr = (int)epkUser["pernr"];
            else
                throw new Exception("Váš účet není propojen s tabulkou Users");

            Tabloid superiorView = db.Tabloid("SuperiorMapping", searchInShared);
            DBItem superiorMapping = superiorView.Select().Where(c => c.Column("pernr").Equal(userPernr)).FirstOrDefault();
            DBItem result;
            Tabloid userView = db.Tabloid("UserView", searchInShared);
            if (superiorMapping != null)
            {
                outputVars["NoSuperior"] = false;
                var superiorPernr = superiorMapping["superior_pernr"];
                var assistantView = db.Tabloid("AssistantMapping", searchInShared);
                var assistantMapping = assistantView.Select().Where(c => c.Column("pernr").Equal(superiorPernr)).FirstOrDefault();
                if (assistantMapping != null)
                {
                    var assistentPernr = assistantMapping["assistant_pernr"];
                    result = userView.Select().Where(c => c.Column("pernr").Equal(assistentPernr)).FirstOrDefault();
                }
                else
                {
                    result = userView.Select().Where(c => c.Column("pernr").Equal(superiorPernr)).FirstOrDefault();
                }
            }
            else
            {
                result = userView.Select().Where(c => c.Column("pernr").Equal(userPernr)).FirstOrDefault();
                outputVars["NoSuperior"] = true;
            }
            result["DisplayName"] = result["vorna"] + " " + result["nachn"];
            outputVars["SuperiorData"] = result;
        }
    }
}
