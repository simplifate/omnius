using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CORE.Models;

namespace CORE.Controllers
{
    public class ConfigController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();

            return View(e.Modules);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Module model)
        {
            DBEntities e =new DBEntities();
            e.Modules.Add(model);

            return RedirectToAction("Details", new { @id = model.Id });
        }

        public ActionResult Details(int id)
        {
            DBEntities e = new DBEntities();

            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            
            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }
        [HttpPost]
        public ActionResult Update(int id, Module model)
        {
            DBEntities e= new DBEntities();
            Module m = e.Modules.SingleOrDefault(x => x.Id == model.Id);
            if (m !=null)
            {
                m.Update(model);
            }

            return RedirectToAction("Details", new { @id = model.Id });
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Module module = e.Modules.SingleOrDefault(m => m.Id == id);

            e.Modules.Remove(module);

            return RedirectToAction("Index");
        }
    }
}