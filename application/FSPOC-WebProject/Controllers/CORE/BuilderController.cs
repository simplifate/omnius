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
    public class BuilderController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();
            if (Request.IsAjaxRequest())
            {
                return PartialView(e.Modules);
            }
            return View(e.Modules);
        }

       
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Module model)
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();
            e.Modules.Add(model);
            e.SaveChanges();

            return RedirectToAction("Details", new { @id = model.Id });
        }

        public ActionResult Details(int id)
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();

            if (Request.IsAjaxRequest())
            {
                return PartialView(e.Modules.SingleOrDefault(m => m.Id == id));
            }
            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }

        public ActionResult Update(int id)
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();
            if (Request.IsAjaxRequest())
            {
                return PartialView(e.Modules.SingleOrDefault(m => m.Id == id));
            }
            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }
        [HttpPost]
        public ActionResult Update(Module model)
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();
            Module m = e.Modules.SingleOrDefault(x => x.Id == model.Id);
            if (m !=null)
            {
                m.Update(model);
            }
            e.SaveChanges();
            return RedirectToAction("Details", new { @id = model.Id });
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = HttpContext.GetCORE().Entitron.GetStaticTables();
            Module module = e.Modules.SingleOrDefault(m => m.Id == id);

            e.Modules.Remove(module);
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}