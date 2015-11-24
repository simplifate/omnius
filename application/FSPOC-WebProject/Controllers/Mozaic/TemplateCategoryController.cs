using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Entitron.Entity;
using FSS.Omnius.Entitron.Entity.Mozaic;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class TemplateCategoryController : Controller
    {
        // GET: TemplateCategory
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();

            return View(e.TemplateCategories);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == id);
            ViewBag.Categories = e.TemplateCategories.Select(x=>x.Name);

            return View(tempCategory);
        }

        public ActionResult Create()
        {
            TemplateCategory tempCategory = new TemplateCategory();

            return View(tempCategory);
        }

        public ActionResult Create(TemplateCategory model)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == model.Id);

            e.TemplateCategories.Add(tempCategory);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == id);

            return View(tempCategory);
        }

        public ActionResult Update(TemplateCategory model)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == model.Id);

            e.TemplateCategories.AddOrUpdate(tempCategory, model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == id);

            e.TemplateCategories.Remove(tempCategory);
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}