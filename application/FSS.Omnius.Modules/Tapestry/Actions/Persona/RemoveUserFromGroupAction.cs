using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RemoveUserFromGroupAction : Action
    {
        public override int Id
        {
            get
            {
                return 4101;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "UserId", "GroupId", "?RecordId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Remove user from group";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
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
            var context = DBEntities.instance;

            int userId, groupId;
            if (vars.ContainsKey("RecordId"))
            {
                int recordId = Convert.ToInt32(vars["RecordId"]);
                userId = recordId % 10000;
                groupId = recordId / 10000;
            }
            else
            {
                userId = Convert.ToInt32(vars["UserId"]);
                groupId = Convert.ToInt32(vars["GroupId"]);
            }
            if (context.UserRoles.Any(c => c.UserId == userId && c.RoleId == groupId))
            {
                context.UserRoles.Remove(context.UserRoles.SingleOrDefault(c => c.UserId == userId && c.RoleId == groupId));
                context.SaveChanges();
            }

        }
    }
}