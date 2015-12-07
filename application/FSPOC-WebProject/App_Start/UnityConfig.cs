using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.DAL;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Service;
using Unity.Mvc5;

namespace FSPOC_WebProject
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {

            var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            container.RegisterType<IRepository<DbSchemeCommit>, DefaultEFRepository<DbSchemeCommit>>();
            container.RegisterType<IDatabaseGenerateService, DatabaseGenerateService>();
            container.RegisterType<IBackupGeneratorService, BackupGeneratorService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}