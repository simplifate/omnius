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

        private const string _AdGroupContainer = "OU=OSS";

        public Persona(CORE.CORE core)
        {
            Name = "Persona";
            _CORE = core;

            ConfigPair pair = _CORE.Entitron.GetStaticTables().ConfigPairs.SingleOrDefault(c => c.Key == "UserCacheExpirationHours");
            _expirationTime = TimeSpan.FromHours(pair != null ? Convert.ToInt32(pair.Value) : 24); // default 24h
        }

        public int GetLoggedCount()
        {
            return _CORE.Entitron.GetStaticTables().Users.Where(u => u.LastLogout == null).Count();
        }

        public User AuthenticateUser(string username)
        {
            User user = GetUser(username);

            if (user != null && user.LastLogout != null)
            {
                user.LastLogin = user.CurrentLogin;
                user.CurrentLogin = DateTime.UtcNow;
                user.LastLogout = null;
                _CORE.Entitron.GetStaticTables().SaveChanges();
            }

            return user;
        }
        public void LogOff(string username)
        {
            User user = _CORE.Entitron.GetStaticTables().Users.Single(u => u.UserName == username);
            user.LastLogout = DateTime.UtcNow;
            _CORE.Entitron.GetStaticTables().SaveChanges();
        }
        public void NotLogOff(string username)
        {
            User user = _CORE.Entitron.GetStaticTables().Users.Single(u => u.UserName == username);
            user.LastLogout = null;
            _CORE.Entitron.GetStaticTables().SaveChanges();
        }

        public User GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            DBEntities context = _CORE.Entitron.GetStaticTables();
            User user = context.Users.SingleOrDefault(u => u.UserName == username);

            return GetUser(user);
        }
        public User GetUser(User user)
        {
            // is in DB
            if (user != null)
            {
                // is local-only
                if (user.isLocalUser)
                    return user;

                // is from AD
                else
                {
                    if (user.localExpiresAt < DateTime.UtcNow)
                    {
                        var userWithGroups = GetUserFromAD(user.UserName);
                        // user not found - was deleted in AD
                        if (userWithGroups == null)
                            return null;
                        // user found in AD
                        SaveToDB(userWithGroups.Item1, userWithGroups.Item2);
                        user = userWithGroups.Item1;
                    }

                    return user;
                }
            }

            // not in db
            else
            {
                var userWithGroups = GetUserFromAD(user.UserName);

                // user doesn't exist
                if (userWithGroups == null)
                    return null;

                // user found in AD
                SaveToDB(userWithGroups.Item1, userWithGroups.Item2);
                return userWithGroups.Item1;
            }
        }
        
        private Tuple<User, List<string>> GetUserFromAD(string username)
        {
            DBEntities context = _CORE.Entitron.GetStaticTables();

            // split username & domain
            int domainIndex = username.IndexOf('\\');
            string serverName = null;
            string onlyName = username;
            if (domainIndex != -1)
            {
                serverName = username.Substring(0, domainIndex).ToLower();
                onlyName = username.Substring(domainIndex + 1);
            }

            // search in AD
            NexusLdapService search = new NexusLdapService();
            if (serverName != null) search.UseServer(serverName);
            JToken ldapResult = search.SearchByLogin(onlyName);

            // no user found
            if (ldapResult == null)
                return null;

            // user attributes
            User user = new User
            {
                UserName = username,
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

                isLocalUser = false,
                localExpiresAt = DateTime.UtcNow + _expirationTime
            };

            // groups
            List<string> groupNames = new List<string>();
            foreach (JToken group in ldapResult["memberof"])
            {
                string groupIdentify = (string)group;
                // get only OSS groups
                if (groupIdentify.Split(',').Contains(_AdGroupContainer))
                {
                    int startI = groupIdentify.IndexOf("CN=") + 3;
                    int EndI = groupIdentify.IndexOf(',', startI);
                    groupNames.Add(groupIdentify.Substring(startI, EndI - startI));
                }
            }

            return new Tuple<User, List<string>>(user, groupNames);
        }

        private void SaveToDB(User user, List<string> groups)
        {
            DBEntities context = _CORE.Entitron.GetStaticTables();
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
    }
}
