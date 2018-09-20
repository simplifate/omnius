using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Persona
{
    public class MasterAD : MasterAuth, IMasterAuth
    {
        public override int Id => 2;
        public override string Name => "AD";
        public override bool AllowLogout => false;

        public override IPersonaAuth CreateAuth(User user)
        {
            return new AuthAD(user);
        }

        public override void RedirectToLogin(HttpContext context)
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        }

        public override void Refresh()
        {
            RefreshUsers();
            RefreshGroups();
        }

        public void RefreshUsers()
        {
            DBEntities context = COREobject.i.Context;
            NexusLdapService service = new NexusLdapService();

            JToken ldapUsers = service.GetUsers();
            foreach (JToken ldapUser in ldapUsers)
            {
                string username = (string)ldapUser["samaccountname"];
                if (ldapUser["samaccountname"] == null)
                    continue;
                if (context.Users.Any(u => u.UserName == username))
                {
                    Logger.Log.Info($"SyncAD: skipping user {username}");
                    continue;
                }

                try
                {
                    User user = new User
                    {
                        UserName = username,
                        DisplayName = string.IsNullOrWhiteSpace((string)ldapUser["displayname"]) ? username : (string)ldapUser["displayname"],
                        Email = (string)ldapUser["mail"],
                        Address = "",
                        Company = "",
                        Department = "",
                        Team = "",
                        Job = "",
                        WorkPhone = "",
                        MobilPhone = "",
                        LastLogin = (long)ldapUser["lastlogon"] != 0 ? DateTime.FromFileTime((long)ldapUser["lastlogon"]) : new DateTime(1970, 1, 1),
                        CurrentLogin = DateTime.UtcNow,

                        ModuleAccessPermission = new ModuleAccessPermission(),

                        AuthTypeId = Id,
                        localExpiresAt = DateTime.UtcNow.AddMonths(1)
                    };
                    context.Users.Add(user);
                }
                catch (Exception ex)
                {
                    throw new Exception($"LDAP: error in creating user '{username}'", ex);
                }
            }
            context.SaveChanges();
        }

        public void RefreshGroups()
        {
            DBEntities context = COREobject.i.Context;
            string groupADServer = WebConfigurationManager.AppSettings[$"Persona_AdGroupServer"];

            NexusLdapService ldap = new NexusLdapService();
            ldap.UseServer(groupADServer);

            // get ADgroup_User from AD
            List<ADgroup_User> rightsLdap = new List<ADgroup_User>();
            foreach (ADgroup group in context.ADgroups.ToList())
            {
                // For ADGroup with added RoleForApplication remove UserRoles
                if (!string.IsNullOrEmpty(group.RoleForApplication))
                {
                    foreach (User_Role userRole in context.Users_Roles.ToList())
                    {
                        if (userRole.ApplicationId == group.ApplicationId && userRole.RoleName == group.RoleForApplication)
                            context.Users_Roles.Remove(userRole);
                    }
                }

                var ADapps = ldap.GetGroups(group.Name);
                if (ADapps.Count() == 0)
                    continue;

                foreach (JToken ADapp in ADapps) // should be only 1
                {
                    foreach (JToken member in ADapp["member"])
                    {
                        // save user with groups
                        User user = AuthAD.getUserAndHisGroupsFromAD(identify: (string)member).Item1;

                        // Add UserRole according to ADGroup
                        if (!string.IsNullOrEmpty(group.RoleForApplication))
                        {
                            User_Role newUserRole = new User_Role();
                            newUserRole.UserId = user.Id;
                            newUserRole.RoleName = group.RoleForApplication;
                            newUserRole.ApplicationId = group.ApplicationId ?? 0;
                            newUserRole.ApplicationName = context.Applications.Find(group.ApplicationId ?? 0).Name;
                            context.Users_Roles.Add(newUserRole);
                        }
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
