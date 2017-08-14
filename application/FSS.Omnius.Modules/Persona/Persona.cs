using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Persona
{
    using CORE;
    using Nexus.Service;
    using Entitron.Entity;
    using Entitron.Entity.CORE;
    using Entitron.Entity.Persona;

    public class Persona : IModule
    {
        private TimeSpan _expirationTime;
        private TimeSpan _autoLogoffAfter;

        private CORE _CORE;

        private const string _AdGroupContainer = "OU=OSS";
        private static Dictionary<string, string> _ADServerMapping = new Dictionary<string, string> { { "ext", "rwe-ext" }, { "rwe", "rwe-cz" } };

        public static UserManager<User, int> userManager;

        public Persona(CORE core)
        {
            _CORE = core;

            ConfigPair pair = _CORE.Entitron.GetStaticTables().ConfigPairs.SingleOrDefault(c => c.Key == "UserCacheExpirationHours");
            _expirationTime = TimeSpan.FromHours(pair != null ? Convert.ToInt32(pair.Value) : 24); // default 24h
            _autoLogoffAfter = TimeSpan.FromHours(10);
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
        
        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            
            DBEntities context = _CORE.Entitron.GetStaticTables();

            User user = context.Users.FirstOrDefault(u => u.Email == email);

            // is in DB
            if (user != null)
            {
                return user;
            }
            // not in db
            else
            {
                return null;
            }
        }
        public void LogOff(User user)
        {
            DBEntities context = DBEntities.instance;
            if (user == null)
                return;

            user.LastLogout = DateTime.UtcNow;
            context.SaveChanges();

            // Log to activity protocol
            var app = context.Applications.Where(a => a.Name == "Aukcnisystem");
            var currentApp = _CORE.Entitron.Application;

           
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

        public User GetUser(string username = null, string identify = null)
        {
            // validate
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(identify))
                return null;
            // init
            DBEntities context = _CORE.Entitron.GetStaticTables();

            // username
            if (!string.IsNullOrWhiteSpace(username))
            {
                // get user
                User user = context.Users.SingleOrDefault(u => u.UserName == username);

                // if it's OK
                if (user != null && (user.isLocalUser || user.localExpiresAt > DateTime.UtcNow))
                    return user;
            }

            // username & identify
            // get from AD
            var userWithGroups = GetUserFromAD(username, identify);

            // user doesn't exist in AD
            if (userWithGroups == null)
                return null;

            // save with groups
            SaveToDB(userWithGroups.Item1, userWithGroups.Item2);
            return userWithGroups.Item1;
        }
        
        private Tuple<User, List<string>> GetUserFromAD(string username = null, string identify = null)
        {
            DBEntities context = _CORE.Entitron.GetStaticTables();

            // split username & domain
            string serverName;
            string onlyName = null;
            // use username
            if (!string.IsNullOrWhiteSpace(username)) 
            {
                int domainIndex = username.IndexOf('\\');
                serverName = null;
                onlyName = username;
                if (domainIndex != -1)
                {
                    serverName = username.Substring(0, domainIndex).ToLower();
                    onlyName = username.Substring(domainIndex + 1);
                }
            }
            // use identify
            else if (!string.IsNullOrWhiteSpace(identify))
            {
                serverName = getUserServer(identify);
            }
            // nothing
            else
                return null;

            // search in AD
            NexusLdapService search = new NexusLdapService();
            if (serverName != null) search.UseServer(serverName);
            JToken ldapResult = (onlyName != null)
                ? search.SearchByLogin(onlyName)
                : search.SearchByIdentify(identify);

            // no user found
            if (ldapResult == null)
                return null;

            // user attributes
            User user = new User
            {
                UserName = username ?? $"{getUserServer(identify).ToUpper()}\\{ldapResult["samaccountname"]}",
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

        public static void RefreshUsersFromWSO(List<User> usersWSO, CORE core)
        {
            var db = core.Entitron.GetStaticTables();
            //iterate all users from WSO
            foreach (User user in usersWSO)
            {
                //if theres already this user. we will update the db
                var databaseUser = db.Users.SingleOrDefault(u => !u.isLocalUser && u.UserName == user.UserName);
                if (databaseUser != null)
                { 
                    databaseUser.UserName = user.UserName;
                    databaseUser.Company = user.Company;
                    databaseUser.MobilPhone = user.MobilPhone;
                    databaseUser.DisplayName = user.DisplayName;
                    databaseUser.Email = user.Email;
                    //Refresh ROLES
                    databaseUser.Roles.Clear();
                    foreach (User_Role role in user.Users_Roles)
                    {
                        databaseUser.Users_Roles.Add(new User_Role { RoleName = role.RoleName, Application = role.Application, User = databaseUser });
                    }
                    //end refresh roles
                    databaseUser.isActive = true;
                    db.SaveChanges();
                }

                //if the user is not in DB, we will add this user to DB
                else
                {
                    db.Users.Add(user);
                }
            }

            //iterate the users in the DB
            foreach (User user in db.Users)
            {
                //if this user is in db but not in the  WSO, set inactive
                if (!usersWSO.Any(u => u.UserName == user.UserName))
                {
                    user.isActive = false;
                }
            }
            db.SaveChanges();       
        }

        public void AutoLogOff()
        {
            DBEntities context = DBEntities.instance;
            DateTime acceptedActionTime = DateTime.UtcNow - _autoLogoffAfter;

            // 20h since last action
            foreach (User user in context.Users.Where(u => u.LastLogout == null && u.CurrentLogin != null && (u.LastAction == null || u.LastAction < acceptedActionTime)).ToList())
            {
                LogOff(user);
                user.LastAction = null;
            }

            context.SaveChanges();
        }

        public User RefreshUser(User user)
        {
            // is local-only
            if (user.isLocalUser || user.localExpiresAt == null)
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

        private string getUserServer(string identify)
        {
            int iStart = 0;
            int iEnd = 0;
            string serverName = null;

            do
            {
                iStart = identify.IndexOf("DC=", iEnd) + 3;
                iEnd = identify.IndexOf(",", iStart);
                // not found
                if (iStart == -1)
                    return null;

                serverName = identify.Substring(iStart, iEnd - iStart);
            }
            while (!_ADServerMapping.ContainsKey(serverName));

            // map if found
            return _ADServerMapping[serverName];
        }
    }
}