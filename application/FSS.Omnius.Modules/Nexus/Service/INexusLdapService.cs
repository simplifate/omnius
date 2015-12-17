using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusLdapService
    {
        JToken SearchByLogin(string login, string baseDN = "", string[] properties = null);
        JToken SearchByEmail(string email, string baseDN = "", string[] properties = null);
        JToken GetGroups(string baseDN = "", string[] properties = null);
        JToken GetUsers(string baseDN = "", string[] properties = null);
        JToken Search(string filter, string baseDN = "", string[] properties = null);
        JToken FindOne(string filter, string baseDN = "", string[] properties = null);
        void UseServer(string serverName);
    }
}
