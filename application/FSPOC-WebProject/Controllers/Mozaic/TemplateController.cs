using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;
using FSS.Omnius.Entitron.Entity.Mozaic;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class TemplateController : Controller
    {
        // GET: Template
        public ActionResult Index()
        {             
            DBEntities e =new DBEntities();

            return View(e.Templates);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == id); ;

            return View(temp);
        }

        public ActionResult Create()
        {
            DBEntities e = new DBEntities();
            Template temp= new Template();

            List<string> categories = new List<string>();
            categories = e.TemplateCategories.Select(x => x.Name).ToList();
            categories.Add("");

            List<int> pages = new List<int>();
            pages = e.Pages.Select(x=>x.Id).ToList();
            pages.Add(-1);

            ViewBag.Categories = categories;
            ViewBag.Pages = pages;

            return View(temp);
        }
        [HttpPost]
        public ActionResult Create(Template model)
        {
            DBEntities e = new DBEntities();
            foreach (Template t in e.Templates)
            {
                if (t.Name == model.Name)
                {
                    TempData["error"] = "Šablona s názvem " + model.Name + " už existuje.";
                    return RedirectToAction("Index");
                }
            }
           
            e.Templates.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == id); ;

            List<string> categories = new List<string>();
            categories = e.TemplateCategories.Select(x => x.Name).ToList();
            categories.Add("");

            List<int> pages = new List<int>();
            pages = e.Pages.Select(x => x.Id).ToList();
            pages.Add(-1);

            ViewBag.Categories = categories;
            ViewBag.Pages = pages;

            return View(temp);
        }
        [HttpPost]
        public ActionResult Update(Template model)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == model.Id);

            e.Templates.Remove(temp);
            e.Templates.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == id); ;

            e.Templates.Remove(temp);
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}