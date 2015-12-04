using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;

namespace FSS.Omnius.Controllers.Master
{
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
                        if (app.ShowInAppManager)
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
        public ActionResult Index()
        {
            ViewData["Apps"] = getAppList();
            return View();
        }
        public ActionResult Details()
        {
            ViewData["Apps"] = getAppList();
            return View();
        }
        public ActionResult Help()
        {
            ViewData["Apps"] = getAppList();
            return View();
        }
    }
}
