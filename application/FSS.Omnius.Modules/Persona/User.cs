using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public partial class User
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /// <summary>
        /// without saving...
        /// </summary>
        /// <param name="groupNames"></param>
        public void UpdateAppRightFromAd(IEnumerable<string> newADgroupNames, DBEntities context)
        {
            // AD groups
            List<ADgroup_User> newADgroups = context.ADgroups.Where(ad => newADgroupNames.Contains(ad.Name)).ToList().Select(ad => new ADgroup_User { ADgroup = ad, User = this }).ToList();

            // DB groups
            List<ADgroup_User> oldADgroups = context.ADgroup_Users.Where(adu => adu.UserId == Id).ToList();

            // update
            ADgroup.RemoveDuplicated(oldADgroups, newADgroups, (a, b) => a.ADgroupId == b.ADgroupId && a.UserId == b.UserId);
            // uncoment on PRODUCTION !!!
            //context.ADgroup_Users.RemoveRange(oldADgroups);
            context.ADgroup_Users.AddRange(newADgroups);
        }

        public bool isAdmin()
        {
            return ADgroup_Users.Any(adu => adu.ADgroup.Name == "Admin");
        }
        public bool canUseAction(int actionId, DBEntities context)
        {
            return Roles.Any(r => r.AppRole.ActionRuleRights.Any(arr => arr.ActionRuleId == actionId));
        }
        public bool HasRole(string roleName, DBEntities context)
        {
            return Roles.Any(r => r.AppRole.Name == roleName);
        }
        public bool IsInGroup(string groupName)
        {
            return ADgroup_Users.Any(adu => adu.ADgroup.Name == groupName);
        }
        public bool canUseModule(string moduleName)
        {
            return ModuleAccessPermission.hasAccess(moduleName);
        }
    }
}
