using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Web;

namespace FSS.Omnius.Modules.Persona
{
    public interface IMasterAuth
    {
        int Id { get; }
        string Name { get; }
        bool AllowLogout { get; }

        IPersonaAuth CreateAuth(User user);
        void RefreshStartup();
        void Refresh();
        
        void RedirectToLogin(HttpContext context);
    }
}
