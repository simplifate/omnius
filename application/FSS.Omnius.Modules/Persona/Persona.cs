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

        public User getUser(string username, string serverName = null)
        {
            // REMOVE ON PRODUCTION !!!
            username = string.IsNullOrWhiteSpace(username) ? "annonymous" : username;
            //username = string.IsNullOrWhiteSpace(username) ? "martin.novak" : username;

            DBEntities e = _CORE.Entitron.GetStaticTables();
            User user = e.Users.SingleOrDefault(u => u.username == username);

            // new user
            if (user == null)
            {
                user = new User();
                user.username = username;
                user.localExpiresAt = DateTime.MinValue;
                e.Users.Add(user);
            }
            // expiration || new user -> get from AD
            if (user.localExpiresAt < DateTime.UtcNow)
            {
                NexusLdapService search = new NexusLdapService();
                if (serverName != null) search.UseServer(serverName);
                JToken result = search.SearchByLogin(username);

                if (result == null)
                    throw new NotAuthorizedException("User not found");

                // user attributes
                user.DisplayName = (string)result["displayname"];
                user.Email = (string)result["mail"];
                user.Address = "";
                user.Company = "";
                user.Department = "";
                user.Team = "";
                user.Job = (string)result["title"];
                user.WorkPhone = "";
                user.MobilPhone = "";
                user.LastLogin = DateTime.FromFileTime((long)result["lastlogon"]);

                // groups
                List<string> groupNames = new List<string>();
                foreach (JToken group in result["memberof"])
                {
                    string groupName = (string)group;
                    int startI = groupName.IndexOf("CN=") + 3;
                    int EndI = groupName.IndexOf(',');
                    groupNames.Add(groupName.Substring(startI, EndI - startI));
                }
                user.UpdateGroupsFromAd(groupNames, e);

                user.localExpiresAt = DateTime.UtcNow + _expirationTime;
                
                e.SaveChanges();
            }

            return user;
        }

        public bool UserCanExecuteActionRule(int ActionRuleId)
        {
            return _CORE.User.Groups.Any(g => g.ActionRuleRights.Any(ar => ar.ActionRuleId == ActionRuleId && ar.Executable));
        }
        public bool isUserAdmin()
        {
            return false;
        }
    }
}
