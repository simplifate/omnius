using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    [PersonaAuthorize(Roles = "Admin", Module = "Mozaic")]
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

            return View(tempCategory);
        }

        public ActionResult Create()
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = new TemplateCategory();

            List<string> categories = new List<string>();
            categories = e.TemplateCategories.Select(x => x.Name).ToList();
            categories.Add("");

            List<string> children = new List<string>();
            children = tempCategory.Children.Select(x => x.Name).ToList();
            children.Add("");

            ViewBag.Categories = categories;
            ViewBag.Children = children;

            return View(tempCategory);
        }
        [HttpPost]
        public ActionResult Create(TemplateCategory model)
        {
            DBEntities e = new DBEntities();

            e.TemplateCategories.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCategory = e.TemplateCategories.SingleOrDefault(x => x.Id == id);

            List<string> categories= new List<string>();
            categories = e.TemplateCategories.Select(x => x.Name).ToList();
            categories.Add("");
            List<string> children = new List<string>();
            children = tempCategory.Children.Select(x => x.Name).ToList();
            children.Add("");

            ViewBag.Categories = categories;
            ViewBag.Children = children;
            return View(tempCategory);
        }

        [HttpPost]
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