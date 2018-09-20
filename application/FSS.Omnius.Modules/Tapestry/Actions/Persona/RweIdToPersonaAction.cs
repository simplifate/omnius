using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RweIdToPersonaAction : Action
    {
        public override int Id => 4109;
        public override string[] InputVar => new string[] { "Id", "?Email", "?Pernr", "?SapId", "?SapId2" };
        public override string Name => "Persona: RWE property to UserId";
        public override string[] OutputVar => new string[] { "Result" };
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            var context = core.AppContext;
            var rweUsersTable = db.Table("Users");

            List<DBItem> results;
            if (vars.ContainsKey("Id"))
            {
                int userId = Convert.ToInt32(vars["Id"]);
                results = rweUsersTable.Select().Where(c => c.Column("id").Equal(userId)).ToList();
            }
            else {
                results = rweUsersTable.Select().Where(c => c.Column("pernr").Equal((string)vars["Pernr"])).ToList();

            }
            if (results.Count > 0)
            {
                string ad_email = (string)results[0]["ad_email"];
                var userObject = context.Users.SingleOrDefault(u => u.Email == ad_email);
                outputVars["Result"] = userObject == null ? 0 : userObject.Id;
            }
            else
            {
                outputVars["Result"] = 0;
            }
        }
    }
}
