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
    class EditUser : Action
    {
        public override int Id
        {
            get
            {
                return 9831;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "UserId", "?Email" };
            }
        }

        public override string Name
        {
            get
            {
                return "Persona: Edit personal information";
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
            var context = COREobject.i.Context;

            int userId = (int)vars["UserId"];
            string email;
            if (vars.ContainsKey("Email"))
            {
                email =  vars["Email"].ToString();
                var user = context.Users.FirstOrDefault(c => c.Id == userId);
                if (user != null)
                {
                    user.Email = email;
                    context.SaveChanges();
                }
            }
        }
    }
}