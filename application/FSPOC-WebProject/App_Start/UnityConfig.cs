using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;
using FSPOC_WebProject.Actions;
using FSPOC_WebProject.Service;
using FSS.FSPOC.Actions.ReservationSystem.Service;
using FSS.FSPOC.BussinesObjects.Actions;
using FSS.FSPOC.BussinesObjects.DAL;
using FSS.FSPOC.BussinesObjects.Entities.Actions;
using FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner;
using FSS.FSPOC.BussinesObjects.Entities.Workflow;
using FSS.FSPOC.BussinesObjects.Service;
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

            container.RegisterType<IDbContext, OmniusDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitWork, UnitWork>();
            container.RegisterType<IWorkflowService, WorkflowService>();
            container.RegisterType<IRepository<Workflow>, DefaultEFRepository<Workflow>>();
            container.RegisterType<IRepository<DbSchemeCommit>, DefaultEFRepository<DbSchemeCommit>>();
            container.RegisterType<IRepository<ActionActionRule>, DefaultEFRepository<ActionActionRule>>();
            container.RegisterType<IDatabaseGenerateService, DatabaseGenerateService>();
            container.RegisterType<IBackupGeneratorService, BackupGeneratorService>();
            container.RegisterType<IFactoryAction, FactoryAction>();
            container.RegisterType<IExecuteActionService, ExecuteActionService>();
            container.RegisterType<IReservationSystemActionProvider, ReservationSystemActionProvider>();
            container.RegisterType<ICommonActionsProvider, CommonActionsProvider>();
            //SMAZAT!!!
            container.RegisterType<ISaveEntity, SaveEntity>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}