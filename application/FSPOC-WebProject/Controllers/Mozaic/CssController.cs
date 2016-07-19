using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Mozaic
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
    public class CssController : Controller
    {
        // GET: Css
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();

            return View(e.Css);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            Css css = e.Css.SingleOrDefault(x => x.Id == id);

            return View(css);
        }

        #region Updates
        // Start update environment
        public ActionResult Update(int id)
        {
            DBEntities e = new DBEntities();
            Css css = e.Css.SingleOrDefault(x => x.Id == id);
            
            return SetUpdateCss(css);
        }
        
        public ActionResult SetUpdateCss(Css css)
        {
            ViewBag.CssValue = css.Value;
            ViewBag.CssName = css.Name;

            return View("Update", css);
        }

        // Save updates
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Css model)
        {
            DBEntities e = new DBEntities();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return View("Update", model);
            }

            e.Css.AddOrUpdate(model);
            e.SaveChanges();

            return View("BaseIndex", e.Css);
        }
        #endregion

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Css css = e.Css.SingleOrDefault(x => x.Id == id);

            e.Css.Remove(css);
            e.SaveChanges();

            return RedirectToAction("Index");
        }

        //Create is like Update - but it is not taking CSS file from DB, it creates new
        
        public ActionResult Create()
        {
            Css newCss = new Css();
            newCss.Name = "New CSS";
            newCss.Value = "";

            return SetUpdateCss(newCss);
        }
    }
}