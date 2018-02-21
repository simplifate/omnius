using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    public partial class Application
    {
        //// return app from application DB
        //[ImportExportIgnore]
        //private Application _similarApp;
        //[ImportExportIgnore]
        //public Application similarApp
        //{
        //    get
        //    {
        //        if (connectionString_schema == null)
        //            return this;

        //        if (_similarApp == null)
        //            _similarApp = DBEntities.appInstance(this).Applications.SingleOrDefault(a => a.Name == Name);
        //        return _similarApp;
        //    }
        //}

        public static IQueryable<Application> getAllowed(Modules.CORE.CORE core, string userName)
        {
            DBEntities context = DBEntities.instance;
            var adGroupIds = context.ADgroups.Where(ad => ad.ADgroup_Users.Any(adu => adu.User.UserName == userName)).Select(ad => ad.Id);

            return
                context.Applications.Where(a =>
                    a.IsPublished
                    && a.IsEnabled
                    && !a.IsSystem
                    && a.ADgroups.Any(ad => adGroupIds.Contains(ad.Id)));
        }

        public static Application SystemApp()
        {
            return
                DBEntities.instance.Applications.FirstOrDefault(a => a.IsSystem);
        }
    }
}
