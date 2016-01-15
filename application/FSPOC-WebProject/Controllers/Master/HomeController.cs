﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Persona;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize]
    public class HomeController : Controller
    {
        private List<Application> getAppList()
        {
            User currentUser = User.GetLogged();
            try
            {
                using (var context = new DBEntities())
                {
                    return context.Applications.Where(a =>
                        a.IsPublished
                        && a.IsEnabled
                        && a.Rights.Any(r => r.Group.Users.Any(u => u.Id == currentUser.Id) && r.Executable)
                    ).ToList();
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
