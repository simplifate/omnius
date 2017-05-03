using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RweIdToPersonaAction : Action
    {
        public override int Id
        {
            get
            {
                return 4109;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Id", "?Email", "?Pernr", "?SapId", "?SapId2" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: RWE property to UserId";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
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
            var rweUsersTable = core.Entitron.GetDynamicTable("Users");
            var context = DBEntities.instance;
           


            List<DBItem> results;
            if (vars.ContainsKey("id"))
            {
                int userId = Convert.ToInt32(vars["Id"]);
                results = rweUsersTable.Select().where(c => c.column("id").Equal(userId)).ToList();
            }
            else {
                results = rweUsersTable.Select().where(c => c.column("pernr").Equal((string)vars["Pernr"])).ToList();

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
