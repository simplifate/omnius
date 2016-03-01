using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace FSS.Omnius.Controllers.CORE
{

    [PersonaAuthorize(Roles = "Admin", Module = "CORE")]
    public class ServiceController : Controller
    {
  
        public void BackupApp()
        {
            var backupService = new FSS.Omnius.Modules.Entitron.Service.BackupGeneratorService();
            backupService.ExportApplication(12);
        }

      
    }
}