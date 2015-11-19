using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;
using FSS.Omnius.Entitron.Entity.CORE;

namespace FSPOC_WebProject.Controllers.Admin
{
    public class COREController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
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
            DBEntities e = new DBEntities();
            e.Modules.Add(model);
            e.SaveChanges();

            return RedirectToAction("Details", new { @id = model.Id });
        }

        public ActionResult Details(int id)
        {
            DBEntities e = new DBEntities();
            
            if (Request.IsAjaxRequest())
            {
                return PartialView(e.Modules.SingleOrDefault(m => m.Id == id));
            }
            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            if (Request.IsAjaxRequest())
            {
                return PartialView(e.Modules.SingleOrDefault(m => m.Id == id));
            }
            return View(e.Modules.SingleOrDefault(m => m.Id == id));
        }
        [HttpPost]
        public ActionResult Update(Module model)
        {
            DBEntities e= new DBEntities();
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
            DBEntities e = new DBEntities();
            Module module = e.Modules.SingleOrDefault(m => m.Id == id);

            e.Modules.Remove(module);
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}