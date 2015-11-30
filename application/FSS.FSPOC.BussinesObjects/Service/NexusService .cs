using FSS.Omnius.Nexus.Gate;

namespace FSS.Omnius.BussinesObjects.Service
{
    public class NexusService : INexusService
    {
        public string searchByLogin(string login)
        {
            Ldap ldap = new Ldap();

            return ldap.SearchByAdLogin(login);
        }
    }
}
