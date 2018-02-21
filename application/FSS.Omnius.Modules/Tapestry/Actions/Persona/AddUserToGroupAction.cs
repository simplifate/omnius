using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class AddUserToGroupAction : Action
    {
        public override int Id => 4100;
        public override string[] InputVar => new string[] { "UserId", "GroupId" };
        public override string Name => "Persona: Add user to group";
        public override string[] OutputVar => new string[0];
        public override int? ReverseActionId => null;
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Application);

            int userId = Convert.ToInt32(vars["UserId"]);
            int roleId = Convert.ToInt32(vars["GroupId"]);
            
            var role = context.AppRoles.SingleOrDefault(ar => ar.Id == roleId);
            
            if (!context.Users_Roles.Any(c => c.UserId == userId && c.RoleName == role.Name && c.ApplicationId == role.ApplicationId))
            {
                context.Users_Roles.Add(new User_Role
                {
                    UserId = userId,
                    RoleName = role.Name,
                    ApplicationId = role.ApplicationId
                });
                context.SaveChanges();
            }
        }
    }
}
