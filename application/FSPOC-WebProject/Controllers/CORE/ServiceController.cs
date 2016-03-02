using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Controllers.CORE
{

    [PersonaAuthorize(Roles = "Admin", Module = "CORE")]
    public class ServiceController : Controller
    {
  
        public ActionResult BackupApp(int id)
        {
            var backupService = new FSS.Omnius.Modules.Entitron.Service.BackupGeneratorService();
            string jsonText = backupService.ExportApplication(id);

            return Content(jsonText);
        }

        public ActionResult RevoverApp() {
            return View();
        }
        
    }
}