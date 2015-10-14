using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mozaic.Models;

namespace Mozaic.Controllers
{
    [FilterIP(allowedIp = "127.0.0.1;::1")]
    public class BuilderController : Controller
    {
        #region Page
        public ActionResult Index(string app, int id = -1)
        {
            DBMozaic e = new DBMozaic();
            return View(e.Pages);
        }

        public ActionResult Create(string app)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(string app, Page model)
        {
            // TODO
        }

        public ActionResult Update(string app, int id)
        {
            DBMozaic e = new DBMozaic();
            Page page = e.Pages.SingleOrDefault(p => p.Id == id);

            if (page == null)
                return RedirectToAction("Index");

            return View(e.Pages);
        }
        [HttpPost]
        public ActionResult Update(string app, int id, Page model)
        {
            DBMozaic e = new DBMozaic();
            Page page = e.Pages.SingleOrDefault(p => p.Id == id);

            if (page == null)
                return RedirectToAction("Index");

            // TODO
        }

        public ActionResult Delete(string app, int id)
        {
            DBMozaic e = new DBMozaic();
            Page page = e.Pages.SingleOrDefault(p => p.Id == id);
            if (page != null)
            {
                e.Pages.Remove(page);
                e.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Template
        public ActionResult TemplateIndex(string app, int id = -1)
        {
            DBMozaic e = new DBMozaic();
            TemplateCategory category = e.TemplateCategories.SingleOrDefault(c => c.Id == id);
            if (category == null)
                category = new TemplateCategory()
                {
                    Children = e.TemplateCategories.Where(c => c.ParentId == null).ToList(),
                    Templates = new List<Template>()
                };

            return View(category);
        }

        public ActionResult TemplateCreate(int categoryId)
        {
            return View(new Template() { CategoryId = categoryId });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TemplateCreate(Template model)
        {
            try
            {
                DBMozaic e = new DBMozaic();
                e.Templates.Add(model);
                e.SaveChanges();

                return RedirectToAction("Index", new { @id = model.CategoryId });
            }
            catch (Exception e)
            {
                return View(model);
            }
        }
        #endregion

        #region Category
        public ActionResult TemplateCategoryCreate(int parentId = -1)
        {
            DBMozaic e = new DBMozaic();
            ViewBag.Categories = e.TemplateCategories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == parentId) }).ToList();

            return View(new TemplateCategory() { ParentId = parentId });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TemplateCategoryCreate(int parrentId, TemplateCategory model)
        {
            return View();
        }
        #endregion
    }
}