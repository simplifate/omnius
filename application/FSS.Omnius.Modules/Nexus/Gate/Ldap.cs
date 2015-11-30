using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace FSS.Omnius.Nexus.Gate
{
    using System.Data.Entity;
    using Modules.Entitron.Entity;
    using System.Collections.Specialized;

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
                string protocol = server.Use_SSL == true ? "LDAPS://" : "LDAP://";

                connection = new DirectoryEntry(protocol + server.Domain_Server);
                connection.AuthenticationType = server.Use_SSL == true ? AuthenticationTypes.SecureSocketsLayer : AuthenticationTypes.Secure;
                connection.Username = server.Bind_User;
                connection.Password = server.Bind_Password;                
            }
            catch(DirectoryServicesCOMException e)
            {
                
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

        private void EnsureConnection()
        {
            if (connection == null) {
                UseServer("default");
            }
        }

        private DirectoryEntry GetRoot(string baseDN = null)
        {
            DirectoryEntry root = connection;
            if (baseDN.Length > 0)
            {
                DirectorySearcher rs = new DirectorySearcher(connection) { Filter = "(distinguishedname=" + baseDN + ")" };
                root = rs.FindOne().GetDirectoryEntry();
            }
            return root;
        }

        #region Users

        public SearchResult SearchByAdLogin(string adLogin, string baseDN = null, string[] properties = null)
        {
            return FindOne("(SAMAccountname=" + adLogin + ")", baseDN, properties);
        }

        public SearchResult SearchByEmail(string email, string baseDN = null, string[] properties = null)
        {
            return FindOne("(Mail=" + email + ")", baseDN, properties);
        }

        public SearchResultCollection GetUsers(string baseDN = "", string[] properties = null)
        {
            return Search("(objectCategory=User)", baseDN, properties);
        }

        #endregion

        #region Group

        public SearchResultCollection GetGroups(string baseDN = "", string[] properties = null)
        {
            return Search("(objectCategory=Group)", baseDN, properties);
        }

        #endregion

        #region misc
           
        public SearchResultCollection Search(string filter, string baseDN = "", string[] properties = null)
        {
            EnsureConnection();
            DirectoryEntry root = GetRoot(baseDN);

            DirectorySearcher search = new DirectorySearcher(root);
            search.Filter = filter;
            search.SearchScope = SearchScope.Subtree;

            if (properties != null) {
                search.PropertiesToLoad.AddRange(properties);
            }

            SearchResultCollection result = search.FindAll();
            return result;
        }

        public SearchResult FindOne(string filter, string baseDN = "", string[] properties = null)
        {
            EnsureConnection();
            DirectoryEntry root = GetRoot(baseDN);

            DirectorySearcher search = new DirectorySearcher(root);
            search.Filter = filter;
            search.SearchScope = SearchScope.Subtree;

            if (properties != null)
            {
                search.PropertiesToLoad.AddRange(properties);
            }

            SearchResult result = search.FindOne();
            return result;
        }

        #endregion
    }
}
