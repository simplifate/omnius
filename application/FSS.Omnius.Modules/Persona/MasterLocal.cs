using System.Web;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Persona
{
    public class MasterLocal : MasterAuth, IMasterAuth
    {
        public override int Id => 0;
        public override string Name => "local";
        public override bool AllowLogout => true;

        public override IPersonaAuth CreateAuth(User user)
        {
            return new AuthLocal(user);
        }

        public override void RedirectToLogin(HttpContext context)
        {
            context.Response.RedirectToRoute(
                       "Persona",
                       new
                       {
                           @controller = "Account",
                           @action = "Login",
                           @returnUrl = context.Request.Url.PathAndQuery
                       }
                   );
        }
    }
}
