using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Tapestry.Actions.Persona
{
    public class UserHasRoleAction : Action
    {
        public override int Id
        {
            get
            {
                return 4106;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Role", "Role2", "Role3", "UserId" };
            }
        }

        public override string Name
        {
            get
            {
                return "User has role";
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
            var context = DBEntities.appInstance(core.Entitron.Application);
            int appId = core.Entitron.AppId;
            string role, role2, role3;
            bool result;

            User user;
            if (vars.ContainsKey("UserId"))
            {
                int userId = (int)vars["UserId"];
                user = context.Users.Find(userId);
            }
            else
            {
                user = core.User;
            }

            if (vars.ContainsKey("Role3"))
            {
                role = (string)vars["Role"];
                role2 = (string)vars["Role2"];
                role3 = (string)vars["Role3"];
                result = user.HasRole(role, appId) || user.HasRole(role2, appId) || user.HasRole(role3, appId);
            }
            else if (vars.ContainsKey("Role2"))
            {
                role = (string)vars["Role"];
                role2 = (string)vars["Role2"];
                result = user.HasRole(role, appId) || user.HasRole(role2, appId);
            }
            else
            {
                role = (string)vars["Role"];
                result = user.HasRole(role, appId);
            }

            outputVars["Result"] = result;
        }
    }
}