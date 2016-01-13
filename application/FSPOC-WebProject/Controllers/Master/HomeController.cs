using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize]
    public class HomeController : Controller
    {
        private List<Application> getAppList()
        {
            try
            {
                using (var context = new DBEntities())
                {
                    var filteredAppList = new List<Application>();

                    foreach (var app in context.Applications)
                    {
                        // TODO: Implement filtering by user privileges
                        if (app.IsPublished && app.IsEnabled)
                            filteredAppList.Add(app);
                    }
                    return filteredAppList;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while loading application list. Exception message: " + ex.Message);
                throw ex;
            }
        }
        private void loadUserInterfaceData()
        {
            ViewData["Apps"] = getAppList();
        }

        public ActionResult Index()
        {
            loadUserInterfaceData();
            return View();
        }
        public ActionResult Details()
        {
            loadUserInterfaceData();
            
            return View();
        }
        public ActionResult Help()
        {
            loadUserInterfaceData();
            return View();
        }
    }
}
