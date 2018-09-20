using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Persona;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    public partial class User : IPersonaAuth
    {
        [NotMapped]
        private IPersonaAuth _auth;

        #region Decorator
        public bool IsAdmin => _auth.IsAdmin;
        public IMasterAuth Auth => _auth.Auth;

        public void Authenticate(MyHttpRequest request)
        {
            _auth.Authenticate(request);
        }

        public void Authorize(bool needsAdmin = false, string moduleName = null, string usernames = null, Application application = null)
        {
            _auth.Authorize(needsAdmin, moduleName, usernames, application);
        }

        public bool CanUseAction(int ActionId)
        {
            return _auth.CanUseAction(ActionId);
        }

        public bool CanUseApplication(Application application)
        {
            return _auth.CanUseApplication(application);
        }

        public bool CanUseBlock(Block block)
        {
            return _auth.CanUseBlock(block);
        }

        public bool CanUseModule(string moduleName)
        {
            return _auth.CanUseModule(moduleName);
        }

        public bool HasRole(string roleName, int applicationId)
        {
            return _auth.HasRole(roleName, applicationId);
        }

        public void Login()
        {
            _auth.Login();
        }

        public void Logout()
        {
            _auth.Logout();
        }
        #endregion

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
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
