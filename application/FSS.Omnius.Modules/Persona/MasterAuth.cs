using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Modules.Persona
{
    public abstract class MasterAuth : IMasterAuth
    {
        public abstract int Id { get; }
        public abstract string Name { get; }
        public abstract bool AllowLogout { get; }

        public abstract IPersonaAuth CreateAuth(User user);

        public virtual void Refresh()
        {
            // do nothing
        }
        public virtual void RefreshStartup()
        {
            if (int.TryParse(WebConfigurationManager.AppSettings[$"Persona_{Name}_UserAutoRefreshHours"], out int UserAutoRefreshHours))
            {
                COREobject core = COREobject.i;
                CrontabTask task = new CrontabTask { Application = core.Context.Applications.Single(a => a.IsSystem), BlockName = "FSS.Omnius.Modules.Persona.MasterAuth", Executor = $"RefreshAnyAuth/int:{Id}", IsActive = true, IsDeleted = false, Name = $"Scheduler {Name}", Schedule = $"after {UserAutoRefreshHours}hour", ScheduleStart = CrontabTask.ScheduleStartAt.taskStart };
                task.Start();
            }
        }
        
        public abstract void RedirectToLogin(HttpContext context);

        private static Dictionary<int, IMasterAuth> _all;
        public static Dictionary<int, IMasterAuth> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new Dictionary<int, IMasterAuth>();
                    var types = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetInterface("FSS.Omnius.Modules.Persona.IMasterAuth") != null && !t.IsAbstract);
                    foreach (Type type in types)
                    {
                        IMasterAuth instance = (IMasterAuth)Activator.CreateInstance(type);
                        _all.Add(instance.Id, instance);
                    }
                }

                return _all;
            }
        }

        public static IPersonaAuth CreateAnyAuth(User user)
        {
            return All[user.AuthTypeId].CreateAuth(user);
        }

        public static void RefreshAnyAuth(int authId)
        {
            All[authId].Refresh();
        }
    }
}
