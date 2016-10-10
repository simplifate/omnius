using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using FSS.Omnius.Modules.Entitron.Entity.Master;

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
        
        public bool isAdmin()
        {
            return ADgroup_Users.Any(adu => adu.ADgroup.Application != null && adu.ADgroup.Application.IsSystem);
        }
        public bool canUseAction(int actionId, DBEntities context)
        {
            int count = context.ActionRuleRights.Count(a => a.ActionRuleId == actionId);
            if (count == 0)
                return true;

            return Roles.Any(r => r.AppRole.ActionRuleRights.Any(arr => arr.ActionRuleId == actionId));
        }
        public bool canUseBlock(string rights,int appId)
        {
            if (string.IsNullOrEmpty(rights))
                return false;

            string[] roles = rights.Split(',');
            foreach (string role in roles)
            {
                if (Roles.Any(r => r.AppRole.ApplicationId == appId && r.AppRole.Name == role))
                    return true;
            }
            return false;
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
            return ModuleAccessPermission == null || ModuleAccessPermission.hasAccess(moduleName);
        }
    }
}
