using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ChangeUserPasswordAction : Action
    {
        public override int Id
        {
            get
            {
                return 4110;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Username", "NewPassword" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Change user password";
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            var context = DBEntities.appInstance(core.Entitron.Application);
            string username = (string)vars["Username"];
            string newPassword = (string)vars["NewPassword"];

            bool userExists = context.Users.Any(c => c.UserName == username);

            if (userExists)
            {
                MembershipUser mu = Membership.GetUser(username);
                string resetedPassword = mu.ResetPassword();
                mu.ChangePassword(resetedPassword, newPassword);

                context.SaveChanges();
            }
        }
    }
}