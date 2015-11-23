using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace Nexus.Gate
{
    public class Ldap
    {
        private DirectoryEntry connection;

        public Ldap()
        {
            Connect();
        }

        private void Connect()
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
