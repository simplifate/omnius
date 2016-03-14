using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;

namespace FSS.Omnius.Controllers.Mozaic
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
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

            ViewBag.Categories = e.TemplateCategories;
            ViewBag.Pages = e.Pages;

            return View(temp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Template model, int cat)
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
            TemplateCategory tempCategory =  e.TemplateCategories.SingleOrDefault(x => x.Id == cat);
            model.CategoryId = cat;
            model.Category = tempCategory;

            e.Templates.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == id); ;
           
            ViewBag.Categories = e.TemplateCategories;
            ViewBag.Pages = e.Pages;

            return View(temp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Template model, int cat)
        {
            DBEntities e = new DBEntities();
            Template temp = e.Templates.SingleOrDefault(x => x.Id == model.Id);

            foreach (Template t in e.Templates)
            {
                if (t.Name == model.Name)
                {
                    TempData["error"] = "Šablona s názvem " + model.Name + " už existuje.";
                    return RedirectToAction("Index");
                }
            }
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == cat);
            model.CategoryId = cat;
            model.Category = tempCategory;

            e.Templates.AddOrUpdate(temp, model);
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