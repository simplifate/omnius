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
        private void loadUserInterfaceData()
        {
            ViewData["Apps"] = getAppList();
            ViewData["UserFullName"] = "Eliška Nováková";
        }

        public ActionResult Index()
        {
            loadUserInterfaceData();
            return View();
        }
        public ActionResult Details()
        {
            loadUserInterfaceData();

            // TODO: Load real data from Nexus
            ViewData["Company"] = "RWE";
            ViewData["Department"] = "?";
            ViewData["Team"] = "?";
            ViewData["Email"] = "?";
            ViewData["WorkPhone"] = "?";
            ViewData["MobilePhone"] = "?";
            ViewData["Address"] = "?";
            ViewData["LastLogin"] = "?";
            return View();
        }
        public ActionResult Help()
        {
            loadUserInterfaceData();
            return View();
        }
    }
}
