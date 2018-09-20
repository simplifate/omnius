using System.Linq;

namespace FSS.Omnius.Modules.Persona
{
    using CORE;
    using Entitron.Entity.Persona;
    using System;
    using System.Linq.Expressions;

    public class Persona : IModule
    {
        public static User GetUser(Expression<Func<User, bool>> selector, bool allowGuest)
        {
            COREobject core = COREobject.i;

            User user = core.Context.Users.SingleOrDefault(selector);
            if (user == null && allowGuest)
                user = core.Context.Users.Single(u => u.UserName == "guest");

            return user;
        }
        public static User GetUser(string username, bool allowGuest)
        {
            return GetUser(u => u.UserName == username, allowGuest);
        }
        public static User GetAuthenticatedUser(Expression<Func<User, bool>> selector, bool allowGuest, MyHttpRequest request)
        {
            User user = GetUser(selector, allowGuest);
            if (user == null)
                throw new LoggedOff("User is not logged!");
            user.Authenticate(request);
            return user;
        }
        public static User GetAuthenticatedUser(string username, bool allowGuest, MyHttpRequest request)
        {
            return GetAuthenticatedUser(u => u.UserName == username, allowGuest, request);
        }
        public static User GetAuthenticatedUserByEmail(string email, bool allowGuest, MyHttpRequest request)
        {
            return GetAuthenticatedUser(u => u.Email == email, allowGuest, request);
        }

        public static int GetLoggedCount()
        {
            return COREobject.i.Context.Users.Count(u => u.LastLogout < u.CurrentLogin);
        }
        
        public static void RefreshStartup()
        {
            foreach (IMasterAuth masterAuth in MasterAuth.All.Values)
            {
                masterAuth.RefreshStartup();
            }
        }
    }
}