using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RemoveUserFromGroupAction : Action
    {
        public override int Id => 4101;
        public override string[] InputVar => new string[] { "UserId", "GroupId", "?RecordId" };
        public override string Name => "Persona: Remove user from group";
        public override string[] OutputVar => new string[0];
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Application);

            int userId = Convert.ToInt32(vars["UserId"]);
            int groupId = Convert.ToInt32(vars["GroupId"]);

            PersonaAppRole role = context.AppRoles.Find(groupId);
            if (context.Users_Roles.Any(c => c.UserId == userId && c.RoleName == role.Name))
            {
                context.Users_Roles.Remove(context.Users_Roles.SingleOrDefault(c => c.UserId == userId && c.RoleName == role.Name));
                context.SaveChanges();
            }

        }
    }
}