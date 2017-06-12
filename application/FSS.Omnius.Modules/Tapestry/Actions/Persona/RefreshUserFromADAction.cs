using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RefreshUserFromADAction : Action
    {
        public override int Id
        {
            get
            {
                return 4102;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Email" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Refresh user from AD";
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

            User targetUser = context.Users.SingleOrDefault(c => c.Email == userEmail);
            if (targetUser != null)
            {
                core.Persona.RefreshUser(targetUser);
            }
            else
            {
                WatchtowerLogger.Instance.LogEvent(
                        $"Refresh z AD - uživatel s emailem \"{userEmail}\" neexistuje",
                        core.User.Id,
                        LogEventType.Tapestry,
                        LogLevel.Warning,
                        false,
                        core.Entitron.AppId
                    );
            }
        }
    }
}