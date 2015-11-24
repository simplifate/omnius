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
        public ActionResult Index(int idCat)
        { 
            DBEntities e =  new DBEntities();
            TemplateCategory tempCat = e.TemplateCategories.SingleOrDefault(x=>x.Id==idCat);
            
            return View(tempCat.Templates);
        }

        public ActionResult Detail(int idCat, int idTemp)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCat = e.TemplateCategories.SingleOrDefault(x => x.Id == idCat);
            Template temp = tempCat.Templates.SingleOrDefault(x => x.Id == idTemp); ;

            return View(temp);
        }

        public ActionResult Create()
        {
            Template temp= new Template();

            return View(temp);
        }

        public ActionResult Create(Template model,int idCat)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCat = e.TemplateCategories.SingleOrDefault(x => x.Id == idCat);
            foreach (Template t in tempCat.Templates)
            {
                if (t.Name == model.Name)
                {
                    //todo return some error message
                }
            }
            tempCat.Templates.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            TemplateCategory tempCategory = new TemplateCategory();
            Template temp = tempCategory.Templates.SingleOrDefault(x => x.Id == id);

            return View(temp);
        }

        public ActionResult Update(Template model, int idCat)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCat = e.TemplateCategories.SingleOrDefault(x => x.Id == idCat);
            Template temp = tempCat.Templates.SingleOrDefault(x => x.Id == model.Id);

            tempCat.Templates.Remove(temp);
            tempCat.Templates.Add(model);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int idTemp, int idCat)
        {
            DBEntities e = new DBEntities();
            TemplateCategory tempCat = e.TemplateCategories.SingleOrDefault(x => x.Id == idCat);
            Template temp = tempCat.Templates.SingleOrDefault(x => x.Id == idTemp); ;

            tempCat.Templates.Remove(temp);
            e.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}