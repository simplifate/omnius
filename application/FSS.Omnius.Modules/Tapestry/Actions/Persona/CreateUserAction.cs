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
    class CreateUserAction : Action
    {
        public override int Id
        {
            get
            {
                return 4104;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Email", "Name", "Surname" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Create user";
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
            string userEmail = (string)vars["Email"];
            string name = (string)vars["Name"];
            string surname = (string)vars["Surname"];

            bool userExists = context.Users.Any(c => c.Email == userEmail);

            if (!userExists)
            {
                var user = new User
                {
                    Email = userEmail,
                    UserName = userEmail,
                    DisplayName = name + " " + surname,
                    SecurityStamp = "b532ea85-8d2e-4ffb-8c64-86e8bfe363d7",
                    localExpiresAt = DateTime.Now.AddYears(1),
                    CurrentLogin = DateTime.Now,
                    LastLogin = DateTime.Now,
                    LastLogout = DateTime.Now,
                    LastAction = DateTime.Now
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}