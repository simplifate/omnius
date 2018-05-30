using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Master")]
    public class ServiceController : Controller
    {
        public FileStreamResult BackupApp(int id)
        {
            Application app = DBEntities.instance.Applications.Find(id);

            var backupService = new BackupGeneratorService(DBEntities.appInstance(app));
            string jsonText = backupService.ExportApplication(id, Request.Form);
            var byteArray = Encoding.UTF8.GetBytes(jsonText);
            var stream = new MemoryStream(byteArray);

            return File(stream, "application/force-download", app.Name + ".json");
        }
        public ActionResult RecoverApp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecoverApp(HttpPostedFileBase file, bool overideTempApp = false)
        {
            // validate
            if (file == null || file.ContentLength == 0 || (file.ContentType != "text/plain" && file.ContentType != "application/octet-stream" && file.ContentType != "application/json"))
                return View();
            
            // get data
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = Encoding.UTF8.GetString(binData);

            // transfer to object
            var service = new RecoveryService();
            
            try
            {
                service.RecoverApplication(result, overideTempApp);
                
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