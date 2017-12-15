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
        public bool canUseBlock(string rights, int appId)
        {
            DBEntities ent = new DBEntities();
            Application app = ent.Applications.SingleOrDefault(a => a.Id == appId);
            if(app.IsAllowedGuests)
            {
                return true;
            }

            if (string.IsNullOrEmpty(rights))
                return true;

            string[] roles = rights.Split(',');
            foreach (string role in roles)
            {
                if (Users_Roles.Any(r => r.ApplicationId == appId && r.RoleName == role))
                    return true;
            }
            return false;
        }
        public bool canUseBlock(string rights, string appName)
        {
            DBEntities ent = new DBEntities();
            Application app = ent.Applications.SingleOrDefault(a => a.Name == appName);
            if (app.IsAllowedGuests)
            {
                return true;
            }

            if (string.IsNullOrEmpty(rights))
                return true;

            string[] roles = rights.Split(',');
            foreach (string role in roles)
            {
                if (Users_Roles.Any(r => r.Application.Name == appName && r.RoleName == role))
                    return true;
            }
            return false;
        }
        public bool HasRole(string roleName, int appId)
        {
            return Users_Roles.Any(r => r.RoleName == roleName && r.ApplicationId == appId);
        }
        public bool IsInGroup(string groupName)
        {
            return ADgroup_Users.Any(adu => adu.ADgroup.Name == groupName);
        }
        public bool canUseModule(string moduleName)
        {
            return ModuleAccessPermission == null || ModuleAccessPermission.hasAccess(moduleName);
        }

        public List<string> GetAppRoles(int appId)
        {
            return Users_Roles.Where(r => r.ApplicationId == appId).Select(r => r.RoleName).ToList();
        }
        public List<string> GetAppRoles(string appName)
        {
            return Users_Roles.Where(r => r.ApplicationName == appName).Select(r => r.RoleName).ToList();
        }
    }
}
