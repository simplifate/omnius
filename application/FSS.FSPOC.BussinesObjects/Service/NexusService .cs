using Nexus.Gate;

namespace FSS.FSPOC.BussinesObjects.Service
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
