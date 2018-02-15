using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using User = FSS.Omnius.Modules.Entitron.Entity.Persona.User;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ChangeUserPasswordAction : Action
    {
        public override int Id => 4110;
        public override string[] InputVar => new string[] { "Username", "NewPassword" };
        public override string Name => "Persona: Change user password";
        public override string[] OutputVar => new string[0];
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Application);
            string username = (string)vars["Username"];
            string newPassword = (string)vars["NewPassword"];

            bool userExists = context.Users.Any(c => c.UserName == username);

            if (userExists)
            {
                User targetUser = context.Users.SingleOrDefault(c => c.UserName == username);
                var userId = targetUser.GetId();

                UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim> store = new UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim>(context);
                UserManager<User, int> userManager = new UserManager<User, int>(store);

                userManager.RemovePassword(userId);
                userManager.AddPassword(userId, newPassword);
            }
        }
    }
}