using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
using System.DirectoryServices;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Persona
{
    [NotMapped]
    public class Persona : Module
    {
        private TimeSpan _expirationTime;
        private CORE.CORE _CORE;

        public Persona(CORE.CORE core)
        {
            Name = "Persona";
            _CORE = core;

            ConfigPair pair = _CORE.Entitron.GetStaticTables().ConfigPairs.SingleOrDefault(c => c.Key == "UserCacheExpirationHours");
            _expirationTime = TimeSpan.FromHours(pair != null ? Convert.ToInt32(pair.Value) : 24); // default 24h
        }

        public int getLoggedCount()
        {
            return _CORE.Entitron.GetStaticTables().Users.Where(u => u.LastLogout == null).Count();
        }

        public User getUser(string username)
        {
            if (_CORE.User != null)
                return _CORE.User;
            
            JToken ldap;
            User user = getUserWithAD(username, out ldap);

            if (ldap != null)
            {
                // groups
                List<string> groupNames = new List<string>();
                foreach (JToken group in ldap["memberof"])
                {
                    string groupName = (string)group;
                    int startI = groupName.IndexOf("CN=") + 3;
                    int EndI = groupName.IndexOf(',', startI);
                    groupNames.Add(groupName.Substring(startI, EndI - startI));
                }
                user.UpdateAppRightFromAd(groupNames, _CORE.Entitron.GetStaticTables());
            }

            _CORE.User = user;
            return user;
        }
        public User getUserWithoutGroups(string username)
        {
            JToken ldap;
            return getUserWithAD(username, out ldap);
        }

        private User getUserWithAD(string username, out JToken ldapResult)
        {
            // REMOVE ON PRODUCTION !!!
            username = string.IsNullOrWhiteSpace(username) ? "annonymous" : username;
            //username = string.IsNullOrWhiteSpace(username) ? "martin.novak" : username;
            
            // init
            DBEntities e = _CORE.Entitron.GetStaticTables();
            User user = e.Users.SingleOrDefault(u => u.UserName == username);
            ldapResult = null;
            
            // new user
            if (user == null)
            {
                user = new User();
                user.UserName = username;
                user.localExpiresAt = DateTime.MinValue;
                e.Users.Add(user);

                ModuleAccessPermission map = new ModuleAccessPermission
                {
                    User = user,
                    Athena = false,
                    Core = false,
                    Cortex = false,
                    Entitron = false,
                    Hermes = false,
                    Master = false,
                    Mozaic = false,
                    Nexus = false,
                    Persona = false,
                    Sentry = false,
                    Tapestry = false,
                    Watchtower = false
                };
                e.ModuleAccessPermissions.Add(map);
            }
            // expiration || new user -> get from AD
            if (user.localExpiresAt < DateTime.UtcNow)
            {
                // split username & domain
                int domainIndex = username.IndexOf('\\');
                string serverName = null;
                if (domainIndex != -1)
                {
                    serverName = username.Substring(0, domainIndex);
                    username = username.Substring(domainIndex + 1);
                }

                // search in AD
                NexusLdapService search = new NexusLdapService();
                if (serverName != null) search.UseServer(serverName);
                ldapResult = search.SearchByLogin(username);

                if (ldapResult == null)
                    throw new NotAuthorizedException("User not found");

                // user attributes
                user.DisplayName = (string)ldapResult["displayname"];
                user.Email = (string)ldapResult["mail"];
                user.Address = "";
                user.Company = "";
                user.Department = "";
                user.Team = "";
                user.Job = (string)ldapResult["title"];
                user.WorkPhone = "";
                user.MobilPhone = "";
                user.LastLogin = DateTime.FromFileTime((long)ldapResult["lastlogon"]);

                user.localExpiresAt = DateTime.UtcNow + _expirationTime;

                e.SaveChanges();
            }

            return user;
        }
    }
}
