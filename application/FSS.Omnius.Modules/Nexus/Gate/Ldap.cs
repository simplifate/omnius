using System.Linq;
using System.DirectoryServices;

namespace FSS.Omnius.Nexus.Gate
{
    using System.Data.Entity;
    using Modules.Entitron.Entity;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using FSS.Omnius.Modules.CORE;

    public enum LdapUACFlags
    {
        password_expired                = 8388608,
        dont_req_preauth                = 4194304,
        use_des_key_only                = 2097152,
        not_delegated                   = 1048576,
        trusted_for_delegation          = 524288,
        smartcard_required              = 262144,
        mns_logon_account               = 131072,
        dont_expire_password            = 65536,
        server_trust_account            = 8192,
        workstation_trust_account       = 4096,
        interdomain_trust_account       = 2048,
        normal_account                  = 512,
        temp_duplicate_account          = 256,
        encrypted_text_pwd_allowed      = 128,
        passwd_cant_change              = 64,
        passwd_notreqd                  = 32,
        lockout                         = 16,
        homedir_required                = 8,
        account_disabled                = 2,
        script                          = 1,
        trusted_to_auth_for_delegation = 16777216
    }

    public class Ldap
    {
        private DirectoryEntry connection;
        private DbSet<Modules.Entitron.Entity.Nexus.Ldap> ldapList;

        public Ldap()
        {
            DBEntities e = COREobject.i.Context;
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
                Logger.Log.Error($"Cannot connect to LDAP: {e.Message}");
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

        public JToken SearchByAdLogin(string adLogin, string baseDN = null, string[] properties = null)
        {
            return FindOne($"(SAMAccountname={adLogin})", baseDN, properties);
        }

        public JToken SearchByEmail(string email, string baseDN = null, string[] properties = null)
        {
            return FindOne($"(Mail={email})", baseDN, properties);
        }
        public JToken SearchByIdentify(string identify, string baseDN = null, string[] properties = null)
        {
            return FindOne($"(distinguishedname={identify})", baseDN, properties);
        }

        public JArray GetUsers(string baseDN = "", string[] properties = null)
        {
            return Search("(objectCategory=User)", baseDN, properties);
        }

        #endregion

        #region Group

        public JArray GetGroups(string CN = "", string baseDN = "", string[] properties = null)
        {
            return Search("(&(objectCategory=Group)(cn="+CN+"))", baseDN, properties);
        }

        #endregion

        #region misc
           
        public JArray Search(string filter, string baseDN = "", string[] properties = null)
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

            return CollectionToJToken(result);
        }

        public JToken FindOne(string filter, string baseDN = "", string[] properties = null)
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
            // not found
            if (result == null)
                return null;

            return ResultToJToken(result);
        }

        private JArray CollectionToJToken(SearchResultCollection collection)
        {
            JArray json = new JArray();
            foreach(SearchResult result in collection) {
                json.Add(ResultToJToken(result));
            }

            return json;
        }

        private JToken ResultToJToken(SearchResult result)
        {
            JToken json = JToken.FromObject(new { });

            foreach (string propName in result.Properties.PropertyNames)
            {
                ResultPropertyValueCollection valueCollection = result.Properties[propName];
                if (valueCollection.Count == 1)
                {
                    json[propName] = JToken.FromObject(valueCollection[0]);
                }
                else if (valueCollection.Count > 1)
                {
                    json[propName] = new JArray();
                    foreach (object value in valueCollection)
                    {
                        ((JArray)json[propName]).Add(JToken.FromObject(value));
                    }
                }
                else
                {
                    json[propName] = null;
                }
            }

            ParseUAC(ref json);

            return json;
        }

        private void ParseUAC(ref JToken json)
        {
            List<string> flags = new List<string>();
            if(json["useraccountcontrol"] != null)
            {
                int temp;
                int uac = (int)json["useraccountcontrol"];
                while(uac > 0)
                {
                    Array flagList = Enum.GetValues(typeof(LdapUACFlags));
                    Array.Reverse(flagList);

                    foreach (var flag in flagList)
                    {
                        temp = uac - (int)flag;
                        if(temp >= 0)
                        {
                            flags.Add(flag.ToString());
                            uac = temp;
                        }
                    }
                }

                json["useraccountcontrol"] = JArray.FromObject(flags);
            }
        }

        #endregion
    }
}
