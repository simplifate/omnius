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
        public ActionResult Index()
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
                    ViewData["Tiles"] = filteredAppList;
                    return View();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while loading application list. Exception message: " + ex.Message);
                return new HttpStatusCodeResult(500);
            }
        }
    }
}
