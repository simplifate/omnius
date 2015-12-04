using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Modules.Persona
{
    [NotMapped]
    public class Persona : Module
    {
        private CORE.CORE _CORE;

        public Persona(CORE.CORE core)
        {
            Name = "Persona";
            _CORE = core;
        }

        public User getUser(string username)
        {
            username = string.IsNullOrWhiteSpace(username) ? "annonymous" : username;

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
            if (user == null || user.localExpiresAt < DateTime.UtcNow)
            {
                // TODO
                user.DisplayName = "Franta Nový";
                user.Email = "franta.novy@rwe.cz";
                user.Company = "FSS";
                user.Department = "Headoffice";
                user.Team = "Leaders";
                user.WorkPhone = "+420 222 222 222";
                user.MobilPhone = "+420 777 777 777";
                user.Address = "Závišova 66";
                user.Job = "Technical administrator";
                user.LastLogin = DateTime.UtcNow;
                user.localExpiresAt = DateTime.UtcNow + TimeSpan.FromDays(1);
                //NexusLdapService search = new NexusLdapService();
                //SearchResult result = search.SearchByLogin("samuel.la");

                //if (result == null)
                //    return null;

                //var prop = result.Properties;
                //user = new User
                //{
                //    username = (string)prop["samaccountname"][0],
                //    DisplayName = (string)prop["displayname"][0],
                //    Job = (string)prop["title"][0],
                //    Mail = (string)prop["mail"][0],
                //    // Groups = 
                //    LastLogon = (DateTime)prop["lastlogon"][0]
                //};

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
