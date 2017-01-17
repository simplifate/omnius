using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class PersonaIdToRweAction : Action
    {
        public override int Id
        {
            get
            {
                return 4108;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "UserId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: UserId to RWE user row";
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
            int userId = (int)vars["UserId"];

            var user = context.Users.Find(userId);
            var results = rweUsersTable.Select().where(c=> c.column("ad_email").Equal(user.Email)).ToList();
            if(results.Count > 0)
            {
                outputVars["Result"] = results[0];
            }
            else
            {
                outputVars["Result"] = null;
            }
        }
    }
}
