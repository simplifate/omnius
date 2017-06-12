using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Newtonsoft.Json;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Master")]
    public class ServiceController : Controller
    {
        public FileStreamResult BackupApp(int id)
        {
            var backupService = new BackupGeneratorService();
            var context = DBEntities.instance;
            string jsonText = backupService.ExportApplication(id);
            var appName = context.Applications.SingleOrDefault(a => a.Id == id).Name;
            var byteArray = Encoding.UTF8.GetBytes(jsonText);
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain",  appName + ".txt");

        }
        public ActionResult RecoverApp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecoverApp(HttpPostedFileBase file)
        {
            // validate
            if (file == null || file.ContentLength == 0 || file.ContentType != "text/plain")
                return View();
            
            // get data
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = Encoding.UTF8.GetString(binData);

            // transfer to object
            var context = DBEntities.instance;
            var service = new RecoveryService();
            
            try
            {
                // get apps
                Application newApp = service.RecoverApplication(result);
                Application dbApp = context.Applications.SingleOrDefault(a => a.Name == newApp.Name);

                // update
                if (dbApp != null)
                    dbApp.UpdateDeep(newApp, context);
                else
                {
                    // set basics value
                    newApp.IsEnabled = false;
                    newApp.IsPublished = false;
                    newApp.DbSchemeLocked = false;

                    context.Applications.Add(newApp);
                    dbApp = newApp;
                }

                // map Id
                var idMapping = dbApp.MapIds(newApp);
                // link object
                dbApp.UpdateEntityLinks(idMapping);
                context.SaveChanges();
                // link Id
                dbApp.UpdateEntityIdLinks(idMapping);
                context.SaveChanges();
                
                return RedirectToRoute("Master", new { @controller = "AppAdminManager", @action = "Index" });
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, Request);
                ViewData["Message"] = ex.Message;
                return View();
            }
        }
    }
}