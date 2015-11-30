using FSS.Omnius.Nexus.Gate;
using System.DirectoryServices;

namespace FSS.Omnius.BussinesObjects.Service
{
    public class NexusService : INexusService
    {
        Ldap ldap;

        public NexusService()
        {
            ldap = new Ldap();
        }

        public void UseServer(string serverName)
        {
            ldap.UseServer(serverName);
        }

        public SearchResult SearchByLogin(string login, string baseDN = "", string[] properties = null)
        {
            return ldap.SearchByAdLogin(login, baseDN, properties);
        }

        public SearchResult SearchByEmail(string email, string baseDN = "", string[] properties = null)
        {
            return ldap.SearchByEmail(email, baseDN, properties);
        }

        public SearchResultCollection GetUsers(string baseDN = "", string[] properties = null)
        {
            return ldap.GetUsers(baseDN, properties);
        }

        public SearchResultCollection GetGroups(string baseDN = "", string[] properties = null)
        {
            return ldap.GetGroups(baseDN, properties);
        }

        public SearchResultCollection Search(string filter, string baseDN = "", string[] properties = null)
        {
            return ldap.Search(filter, baseDN, properties);
        }

        public SearchResult FindOne(string filter, string baseDN = "", string[] properties = null)
        {
            return ldap.FindOne(filter, baseDN, properties);
        }
    }
}
