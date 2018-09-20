using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Nexus.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Persona
{
    public class AuthAD : PersonaAuth, IPersonaAuth
    {
        public AuthAD(User user) : base(user)
        {
            _user = user;
        }

        public override bool IsAdmin => _user.ADgroup_Users.Any(adu => adu.ADgroup.Application != null && adu.ADgroup.Application.IsSystem);

        public override void Authenticate(MyHttpRequest request)
        {
            base.Authenticate(request);

            // check expiration time etc.
            if (_user.localExpiresAt < DateTime.UtcNow)
            {
                if (!RefreshUser())
                    throw new NotAuthorizedException("AD has no longer this user");
            }
        }

        /// <summary>
        /// Check AD group for this application
        /// </summary>
        public override bool CanUseApplication(Application application)
        {
            return
                _user.ADgroup_Users.Any(adu => adu.ADgroup.ApplicationId == application.Id);
        }

        /// <summary>
        /// Has role or has AD group with role
        /// </summary>
        public override bool HasRole(string roleName, int applicationId)
        {
            return
                base.HasRole(roleName, applicationId)
                || _user.ADgroup_Users.Any(adu => adu.ADgroup.RoleForApplication == roleName);
        }

        /// <summary>
        /// Return false if refresh was not successful (user was deleted or server is down)
        /// </summary>
        private bool RefreshUser()
        {
            (User user, List<string> groups) = getUserAndHisGroupsFromAD(_user.UserName);

            if (user == null)
                return false;
            
            _user.Update(user);
            _user.localExpiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(int.TryParse(WebConfigurationManager.AppSettings["Persona_AdExpirationTimeMin"], out int expirationTimeMinutes) ? expirationTimeMinutes : 60 * 24);

            SaveToDB(_user, groups);
            return true;
        }
        internal static (User, List<string>) getUserAndHisGroupsFromAD(string userName = null, string identify = null)
        {
            DBEntities context = COREobject.i.Context;

            // split userName & domain
            string serverName;
            string onlyName = null;
            // use userName
            if (!string.IsNullOrWhiteSpace(userName))
            {
                int domainIndex = userName.IndexOf('\\');
                serverName = null;
                onlyName = userName;
                if (domainIndex != -1)
                {
                    serverName = userName.Substring(0, domainIndex).ToLower();
                    onlyName = userName.Substring(domainIndex + 1);
                }
            }
            // use identify
            else if (!string.IsNullOrWhiteSpace(identify))
            {
                serverName = getUserServer(identify);
            }
            // nothing
            else
                return (null, null);

            // search in AD
            NexusLdapService search = new NexusLdapService();
            if (serverName != null) search.UseServer(serverName);
            JToken ldapResult = (onlyName != null)
                ? search.SearchByLogin(onlyName)
                : search.SearchByIdentify(identify);

            // no user found
            if (ldapResult == null)
                return (null, null);

            // user attributes
            User user = new User
            {
                UserName = userName ?? $"{getUserServer(identify).ToUpper()}\\{ldapResult["samaccountname"]}",
                DisplayName = (string)ldapResult["displayname"],
                Email = (string)ldapResult["mail"],
                Address = "",
                Company = "",
                Department = "",
                Team = "",
                Job = (string)ldapResult["title"],
                WorkPhone = "",
                MobilPhone = "",
                LastLogin = DateTime.FromFileTime((long)ldapResult["lastlogon"]),
                CurrentLogin = DateTime.UtcNow,

                ModuleAccessPermission = new ModuleAccessPermission(),

                AuthTypeId = new MasterAD().Id,
                localExpiresAt = DateTime.UtcNow
            };

            // groups
            List<string> groupNames = new List<string>();
            foreach (JToken group in ldapResult["memberof"])
            {
                string groupIdentify = (string)group;

                int startI = groupIdentify.IndexOf("CN=") + 3;
                int EndI = groupIdentify.IndexOf(',', startI);
                groupNames.Add(groupIdentify.Substring(startI, EndI - startI));
            }

            return (user, groupNames);
        }
        private static void SaveToDB(User user, List<string> groups)
        {
            DBEntities context = COREobject.i.Context;
            User dbUser = context.Users.SingleOrDefault(u => u.UserName == user.UserName);

            // update user
            if (dbUser != null)
            {
                dbUser.Update(user);
                context.ADgroup_Users.RemoveRange(context.ADgroup_Users.Where(adu => adu.UserId == dbUser.Id));
            }
            // new user
            else
            {
                dbUser = user;
                context.Users.Add(dbUser);
            }

            // groups
            foreach (string groupName in groups)
            {
                ADgroup group = context.ADgroups.SingleOrDefault(g => g.Name == groupName);
                if (group == null)
                {
                    group = new ADgroup
                    {
                        Name = groupName,
                        Application = context.Applications.SingleOrDefault(a => a.Name == groupName)
                    };
                }

                context.ADgroup_Users.Add(new ADgroup_User
                {
                    User = dbUser,
                    ADgroup = group
                });
            }

            // save
            context.SaveChanges();
        }
        private static string getUserServer(string identify)
        {
            int iStart = 0;
            int iEnd = 0;

            iStart = identify.IndexOf("DC=", iEnd) + 3;
            iEnd = identify.IndexOf(",", iStart);
            // not found
            if (iStart == -1)
                return null;

            return identify.Substring(iStart, iEnd - iStart);
        }
    }
}
