using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System;

namespace FSS.Omnius.Modules.Persona
{
    public class AuthWSO : PersonaAuth, IPersonaAuth
    {
        public AuthWSO(User user) : base(user)
        { }

        public override bool IsAdmin => throw new NotImplementedException();
    }
}
