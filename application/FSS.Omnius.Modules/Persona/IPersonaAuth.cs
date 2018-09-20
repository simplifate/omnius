namespace FSS.Omnius.Modules.Persona
{
    using Entitron.Entity.Master;
    using FSS.Omnius.Modules.CORE;
    using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
    using System.Web.Mvc;

    public interface IPersonaAuth
    {
        bool IsAdmin { get; }
        IMasterAuth Auth { get; }

        void Authenticate(MyHttpRequest request);
        void Authorize(bool needsAdmin = false, string moduleName = null, string usernames = null, Application application = null);

        bool CanUseAction(int actionId);
        bool CanUseBlock(Block block);
        bool CanUseModule(string moduleName);
        bool CanUseApplication(Application application);

        bool HasRole(string roleName, int applicationId);

        void Login();
        void Logout();
    }
}
