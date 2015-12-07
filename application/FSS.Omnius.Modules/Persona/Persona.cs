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

namespace FSS.Omnius.Modules.Persona
{
    [NotMapped]
    public class Persona : Module
    {
        private TimeSpan _expirationTime = TimeSpan.FromDays(1);
        private CORE.CORE _CORE;

        public Persona(CORE.CORE core)
        {
            Name = "Persona";
            _CORE = core;
        }

        public User getUser(string username)
        {
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
                SearchResult result = search.SearchByLogin(username);

                if (result == null)
                    throw new NotAuthorizedException("User not found");

                var prop = result.Properties;
                user.DisplayName = (string)prop["displayname"][0];
                user.Email = (string)prop["mail"][0];
                user.Address = "";
                user.Company = "";
                user.Department = "";
                user.Team = "";
                user.Job = (string)prop["title"][0];
                user.WorkPhone = "";
                user.MobilPhone = "";
                user.LastLogin = (DateTime)prop["lastlogon"][0];

                user.localExpiresAt = DateTime.UtcNow + _expirationTime;
                
                e.SaveChanges();
            }

            return user;
        }

        public bool UserCanExecuteActionRule(int ActionRuleId)
        {
            return _CORE.ActiveUser.Groups.Any(g => g.ActionRights.Any(ar => ar.ActionId == ActionRuleId && ar.Executable));
        }
        public bool isUserAdmin()
        {
            return false;
        }
    }
}
