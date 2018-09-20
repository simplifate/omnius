using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System;
using System.Linq;
using System.Web.Configuration;

namespace FSS.Omnius.Modules.Persona
{
    public abstract class PersonaAuth : IPersonaAuth
    {
        public PersonaAuth(User user)
        {
            _user = user;
        }

        protected User _user;
        public IMasterAuth Auth => MasterAuth.All[_user.AuthTypeId];

        public abstract bool IsAdmin { get; }

        public virtual void Authenticate(MyHttpRequest request)
        {
            if (!bool.TryParse(WebConfigurationManager.AppSettings["Persona_CheckIpAndCookie"], out bool checkIp) || checkIp)
            {
                if (_user.LastIp != request.UserHostAddress)
                    throw new NotAuthorizedException("Last logged IP address changed");

                if (_user.LastAppCookie != request.Cookies[".AspNet.ApplicationCookie"].Value)
                    throw new NotAuthorizedException("Last cookie changed");
            }

            // Auto logoff - too long inactivity
            if (_user.LastAction == null || (int.TryParse(WebConfigurationManager.AppSettings["Persona_AutoLogoutAfterMin"], out int autoLogoutMin) && _user.LastAction < DateTime.UtcNow - TimeSpan.FromMinutes(autoLogoutMin)))
                throw new LoggedOff("Logged off for inactivity");

            _user.LastAction = DateTime.UtcNow;
        }
        public void Authorize(bool needsAdmin = false, string moduleName = null, string usernames = null, Application application = null)
        {
            if (needsAdmin && !IsAdmin)
                throw new NotAuthorizedException($"Admin rights required!");

            if (moduleName != null && !CanUseModule(moduleName))
                throw new NotAuthorizedException($"Access to module {moduleName} required!");

            if (!string.IsNullOrEmpty(usernames) && !usernames.Split(' ').Contains(_user.UserName))
                throw new NotAuthorizedException($"This user is not allowed!");

            if (application != null && !CanUseApplication(application))
                throw new NotAuthorizedException($"This user cannot use this application!");
        }

        public virtual bool CanUseAction(int actionId)
        {
            return _user.Users_Roles.Any(r => r.AppRole.ActionRuleRights.Any(arr => arr.ActionRuleId == actionId));
        }
        public virtual bool CanUseBlock(Block block)
        {
            Application application = block.WorkFlow.Application;
            if (!CanUseApplication(application))
                return false;

            if (string.IsNullOrEmpty(block.RoleWhitelist))
                return true;

            foreach (string role in block.RoleWhitelist.Split(','))
            {
                if (_user.Users_Roles.Any(r => r.Application.Id == application.Id && r.RoleName == role))
                    return true;
            }

            return false;
        }
        public virtual bool CanUseModule(string moduleName)
        {
            return _user.ModuleAccessPermission.hasAccess(moduleName);
        }
        public virtual bool CanUseApplication(Application application)
        {
            // guest
            if (_user.UserName == "guest")
                return application.IsAllowedGuests;

            // app is allowed for all (not for guest)
            if (application.IsAllowedForAll)
                return true;

            // user has access
            if (_user.UsersApplications.Any(ua => ua.ApplicationId == application.Id))
                return true;

            return false;
        }

        public virtual bool HasRole(string roleName, int applicationId)
        {
            return _user.Users_Roles.Any(r => r.RoleName == roleName && r.ApplicationId == applicationId);
        }

        public virtual void Login()
        {
            _user.LastLogin = _user.CurrentLogin;
            _user.CurrentLogin = DateTime.UtcNow;
            _user.LastAction = DateTime.UtcNow;
        }
        public virtual void Logout()
        {
            _user.LastIp = "";
            _user.LastAppCookie = "";

            _user.LastLogout = DateTime.UtcNow;
            COREobject.i.Context.SaveChanges();
        }
    }
}
