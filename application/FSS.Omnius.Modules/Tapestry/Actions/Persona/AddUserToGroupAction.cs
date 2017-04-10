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
    class AddUserToGroupAction : Action
    {
        public override int Id
        {
            get
            {
                return 4100;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "UserId", "GroupId" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Add user to group";
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
            int userId = Convert.ToInt32(vars["UserId"]);
            int groupId = Convert.ToInt32(vars["GroupId"]);
            var context = DBEntities.instance;
            
            if (!context.UserRoles.Any(c => c.UserId == userId && c.RoleId == groupId))
            {
                context.UserRoles.Add(new User_Role
                {
                    UserId = userId,
                    RoleId = groupId
                });
                context.SaveChanges();
            }
        }
    }
}
