using System;
using System.Web.Mvc;
using FSS.Omnius.Modules.Compass.Service;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Linq;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Controllers.Compass
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Compass")]
    public class CompassController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string search)
        {
            this.ViewBag.Search = search;

            //DBEntities e = COREobject.i.Context;
            
            //e.FileMetadataRecords.Add(new FileMetadata() { AppFolderName = "sebela", Filename = "doc.pdf", WebDavServer = e.WebDavServers.First(), TimeChanged = DateTime.Now, TimeCreated = DateTime.Now });
            //e.SaveChanges();
            
            //IElasticService service = new ElasticService();

            //service.Index(e.FileMetadataRecords.Where(a => a.AppFolderName == "sebela").ToList());

            //var r = service.Search(search);


            return View();
        }

        [HttpPost]
        public ActionResult Index_App(string search)
        {
            COREobject core = COREobject.i;

            this.ViewBag.Search = search;
            
            this.ViewBag.AppName = core.Application.Name;

            //DBEntities e = COREobject.i.Context;

            //e.FileMetadataRecords.Add(new FileMetadata() { AppFolderName = "sebela", Filename = "doc.pdf", WebDavServer = e.WebDavServers.First(), TimeChanged = DateTime.Now, TimeCreated = DateTime.Now });
            //e.SaveChanges();

            //IElasticService service = new ElasticService();

            //service.Index(e.FileMetadataRecords.Where(a => a.AppFolderName == "sebela").ToList());

            //var r = service.Search(search);


            return View("Index");
        }

        public JsonResult loadData(string search, string appName)
        {
            if (string.IsNullOrWhiteSpace(appName))
                appName = null;

            IElasticService service = new ElasticService();
            var r = service.Search(search, appName);

            return Json(new { data = r.Select(a => new {
                Id = a.Id,
                TimeChanged = a.TimeChanged.ToString(),
                TimeCreated = a.TimeCreated.ToString(),
                Version = a.Version,
                FileName = a.Filename,
                AppFolderName = a.AppFolderName,
                Highlights = string.Join(" </br> ", a.Highlights) }).ToArray() }, 
                JsonRequestBehavior.AllowGet);

        }
    }
}
