using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Watchtower;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Persona : ActionManager
    {
        [Action(4100, "Persona: Add user to group")]
        public static void AddUserToGroup(COREobject core, int UserId, int GroupId)
        {
            var context = core.Context; //  DBEntities.appInstance(core.Application);
            
            var role = context.AppRoles.SingleOrDefault(ar => ar.Id == GroupId);

            if (!context.Users_Roles.Any(c => c.UserId == UserId && c.RoleName == role.Name && c.ApplicationId == role.ApplicationId))
            {
                context.Users_Roles.Add(new User_Role
                {
                    UserId = UserId,
                    RoleName = role.Name,
                    ApplicationId = role.ApplicationId
                });
                context.SaveChanges();
            }
        }

        [Obsolete] // no needed anymore
        [Action(4102, "Persona: Refresh user from AD")]
        public static void RefreshUserFromAD(COREobject core, string Email)
        {
            //var context = core.AppContext;

            //User targetUser = context.Users.SingleOrDefault(c => c.Email == Email);
            //if (targetUser != null)
            //{
            //    Modules.Persona.Persona.RefreshUser(targetUser);
            //}
            //else
            //{
            //    OmniusWarning.Log($"Refresh z AD - uživatel s emailem \"{Email}\" neexistuje", OmniusLogSource.Tapestry, core.Application, core.User);
            //}
        }

        [Action(4104, "Persona: Create user")]
        public static void CreateUser(COREobject core, string Email, string Name, string Surname)
        {
            var context = core.AppContext;

            bool userExists = context.Users.Any(c => c.Email == Email);

            if (!userExists)
            {
                var user = new User
                {
                    Email = Email,
                    UserName = Email,
                    DisplayName = Name + " " + Surname,
                    SecurityStamp = "b532ea85-8d2e-4ffb-8c64-86e8bfe363d7",
                    localExpiresAt = DateTime.Now.AddYears(1),
                    CurrentLogin = DateTime.Now,
                    LastLogin = DateTime.Now,
                    LastLogout = DateTime.Now,
                    LastAction = DateTime.Now
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        [Obsolete] // no needed anymore
        [Action(4105, "User auto logoff")]
        public static void AutoLogoff(COREobject core)
        {
            //Modules.Persona.Persona.AutoLogOff();
        }

        [Action(4106, "User has role", "Result")]
        public static bool UserHasRole(COREobject core, string[] Role, int? UserId = null)
        {
            var context = core.Context;

            User user = UserId != null
                ? context.Users.Find(UserId.Value)
                : core.User;

            return Role.Any(r => user.HasRole(r, core.Application.Id));
        }

        [Action(4101, "Persona: Remove user from group")]
        public static void RemoveUserFromGroup(COREobject core, int? UserId = null, int? GroupId = null, int? RecordId = null)
        {
            var context = core.AppContext;

            if (RecordId != null)
            {
                UserId = RecordId.Value % 10000;
                GroupId = RecordId.Value / 10000;
            }

            PersonaAppRole role = context.AppRoles.Find(GroupId);
            if (context.Users_Roles.Any(c => c.UserId == UserId && c.RoleName == role.Name))
            {
                context.Users_Roles.Remove(context.Users_Roles.SingleOrDefault(c => c.UserId == UserId && c.RoleName == role.Name));
                context.SaveChanges();
            }
        }

        [Action(4110, "Persona: Change user password")]
        public static void ChangeUserPassword(COREobject core, string Username, string NewPassword)
        {
            var context = core.AppContext;

            bool userExists = context.Users.Any(c => c.UserName == Username);

            if (userExists)
            {
                User targetUser = context.Users.SingleOrDefault(c => c.UserName == Username);
                var userId = targetUser.GetId();

                UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim> store = new UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim>(context);
                UserManager<User, int> userManager = new UserManager<User, int>(store);

                userManager.RemovePassword(userId);
                userManager.AddPassword(userId, NewPassword);
            }
        }

        [Action(4111, "Persona: Record dates of deleted users", "Result")]
        public static List<DBItem> RecordDatesDeleted(COREobject core, List<DBItem> OldData, List<DBItem> NewData)
        {
            // List of newly removed users from AD
            List<DBItem> removedUsers = new List<DBItem>();

            for (int i = 0; i < OldData.Count; i++)
            {
                DBItem oldUser = OldData[i];
                string sapid1 = (string)oldUser["sapid1"];

                // Remove old active users, so oldData contains only deleted users in the end
                if (!(NewData.Any(c => (string)c["sapid1"] == sapid1)))
                    removedUsers.Add(oldUser);
            }

            foreach (DBItem removedUser in removedUsers)
            {
                string sapid1 = (string)removedUser["sapid1"];
                var user = core.Context.Users.SingleOrDefault(c => c.UserName == sapid1 && c.DeletedBySync == null);

                if (user != null)
                    user.DeletedBySync = DateTime.Now;
            }

            core.Context.SaveChanges();
            return OldData;
        }

        [Action(9831, "Persona: Edit personal information")]
        public static void EditUser(COREobject core, int UserId, string Email)
        {
            var context = core.Context;
            
            var user = context.Users.FirstOrDefault(c => c.Id == UserId);
            if (user != null)
            {
                user.Email = Email;
                context.SaveChanges();
            }
        }
    }
}
