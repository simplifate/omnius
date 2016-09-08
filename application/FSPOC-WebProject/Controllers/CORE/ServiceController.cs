using System;
using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using System.IO;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Text;
using System.Web;
using FSS.Omnius.Modules.Entitron.Service;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;

namespace FSS.Omnius.Controllers.CORE
{

    [PersonaAuthorize(NeedsAdmin = true, Module = "CORE")]
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
            //if everything is ok
            if (file != null && file.ContentLength > 0 && file.ContentType == "text/plain") {
                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes((int)file.InputStream.Length);

                string result = Encoding.UTF8.GetString(binData);

                //Now we use recover service
                var service = new RecoveryService();
                Application app = service.RecoverApplication(result);
                app.IsEnabled = false;
                app.IsPublished = false;
                app.BuildLocked = false;

                Dictionary<int, MozaicEditorPage> pageMapping = new Dictionary<int, MozaicEditorPage>();
                foreach(var page in app.MozaicEditorPages)
                {
                    pageMapping.Add(page.Id, page);
                }

                try
                {
                    var context = HttpContext.GetCORE().Entitron.GetStaticTables();
                    context.Applications.Add(app);
                    context.SaveChanges();

                    foreach (var meta in app.TapestryDesignerMetablocks)
                    {
                        foreach (var block in meta.Blocks)
                        {
                            foreach (var commit in block.BlockCommits)
                            {
                                List<int> newPageIds = new List<int>();
                                List<int> pageIds = commit.AssociatedPageIds.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                                foreach(int pageId in pageIds)
                                {
                                    newPageIds.Add(pageMapping[pageId].Id);
                                }

                                commit.AssociatedPageIds = string.Join(",", newPageIds.Select(i => i.ToString()).ToList());
                            }
                        }
                    }
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(ex, Request);
                    ViewData["Message"] = ex.Message;
                    return View();
                }
            }
            return View();
        }
    }
}