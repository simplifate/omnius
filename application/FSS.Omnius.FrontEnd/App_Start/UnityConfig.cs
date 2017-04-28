using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.DAL;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Service;
using Unity.Mvc5;
using Microsoft.AspNet.Identity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Microsoft.AspNet.Identity.EntityFramework;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Web;
using FSS.Omnius.FrontEnd.Controllers.Persona;

namespace FSS.Omnius.FrontEnd
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {

            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            //container.RegisterType<DbContext, DBEntities>();
            container.RegisterType<IUserStore<User, int>, UserStore<User, Iden_Role, int, UserLogin, Iden_User_Role, UserClaim>>();
            container.RegisterType<UserManager<User, int>>(new HierarchicalLifetimeManager());
            container.RegisterType<IOwinContext>(new InjectionFactory(c => c.Resolve<HttpContextBase>().GetOwinContext()));
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(c => c.Resolve<IOwinContext>().Authentication));
            container.RegisterType<AccountController>(new InjectionConstructor());
            
            container.RegisterType<IRepository<DbSchemeCommit>, DefaultEFRepository<DbSchemeCommit>>();
            container.RegisterType<IDatabaseGenerateService, DatabaseGenerateService>();
            container.RegisterType<IBackupGeneratorService, BackupGeneratorService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}