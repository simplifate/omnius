using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace FSS.Omnius.Nexus.Gate
{
    using System.Data.Entity;
    using FSS.Omnius.Modules.Entitron.Entity;

    public class Ldap
    {
        private DirectoryEntry connection;
        private DbSet<Modules.Entitron.Entity.Nexus.Ldap> ldapList;

        public Ldap()
        {
            DBEntities e = new DBEntities();
            ldapList = e.Ldaps;
        }

        private void Connect(Modules.Entitron.Entity.Nexus.Ldap server)
        {
            try {
                //connection = new DirectoryEntry("LDAP://test.fss.com", "CN=Kerberos,OU=Users,OU=FSS,DC=test,DC=fss,DC=com", "FssSecret1.");
                connection = new DirectoryEntry("LDAP://192.168.1.24/OU=FSS,DC=test,DC=fss,DC=com");
                connection.AuthenticationType = AuthenticationTypes.Secure;
                connection.Username = "Kerberos";
                connection.Password = "FssSecret1.";
                
            }
            catch(DirectoryServicesCOMException e)
            {
                var a = e;
            }
        }

        public void UseServer(string server)
        {
            Modules.Entitron.Entity.Nexus.Ldap serverModel;
            if (server == "default") {
                serverModel = ldapList.SingleOrDefault(e => e.Is_Default == true);
            }
            else {
                serverModel = ldapList.SingleOrDefault(e => e.Domain_Ntlm == server || e.Domain_Kerberos == server || e.Domain_Server == server);
            }

            Connect(serverModel);
        }

        public string SearchByAdLogin(string adLogin)
        {
            DirectorySearcher search = new DirectorySearcher(connection);
            search.Filter = "(SAMAccountname=" + adLogin + ")";

            SearchResult user = search.FindOne();
            int a = 1 + 1;
            return String.Empty;
        }
    }
}
