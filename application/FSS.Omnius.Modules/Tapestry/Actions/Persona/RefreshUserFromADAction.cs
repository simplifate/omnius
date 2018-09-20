using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Watchtower;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [Obsolete]
    [OtherRepository]
    class RefreshUserFromADAction : Action
    {
        public override int Id => 4102;
        public override string[] InputVar => new string[] { "Email" };
        public override string Name => "Persona: Refresh user from AD";
        public override string[] OutputVar => new string[0];
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            //COREobject core = COREobject.i;
            //DBEntities context = core.Context;

            //string userEmail = (string)vars["Email"];

            //User targetUser = context.Users.SingleOrDefault(c => c.Email == userEmail);
            //if (targetUser != null)
            //{
            //    Modules.Persona.Persona.RefreshUser(targetUser);
            //}
            //else
            //{
            //    OmniusWarning.Log($"Refresh z AD - uživatel s emailem \"{userEmail}\" neexistuje", OmniusLogSource.Tapestry, core.Application, core.User);
            //}
        }
    }
}