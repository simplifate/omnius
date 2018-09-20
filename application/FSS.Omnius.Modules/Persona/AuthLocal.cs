using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Persona
{
    public class AuthLocal : PersonaAuth, IPersonaAuth
    {
        public AuthLocal(User user) : base(user)
        { }
        
        public override bool IsAdmin => _user.UsersApplications.Any(ua => ua.Application.IsSystem);
    }
}
