using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using System;

namespace FSS.Omnius.Controllers.Mozaic
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
    public class CssController : Controller
    {
        // GET: Css
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;

            return View(e.Css);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            Css css = e.Css.SingleOrDefault(x => x.Id == id);

            return View(css);
        }

        #region Updates
        // Start update environment
        public ActionResult Update(int id)
        {
            DBEntities e = DBEntities.instance;
            Css css = e.Css.SingleOrDefault(x => x.Id == id);
            
            return SetUpdateCss(css);
        }
        
        public ActionResult SetUpdateCss(Css css)
        {
            ViewBag.CssValue = css.Value;
            ViewBag.CssName = css.Name;

            return PartialView("Update", css);
        }

        // Save updates
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Css model)
        {
            DBEntities e = DBEntities.instance;

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return View("Update", model);
            }

            e.Css.AddOrUpdate(model);
            e.SaveChanges();

            // SassCompiler compiler = new SassCompiler();
            // var cssText = compiler.Compile(model.Value, OutputStyle.Compressed, true);
            string cssText = "";

            string path = GetCssFilePath(model);
            System.IO.File.WriteAllText(path, cssText);

            return View("BaseIndex", e.Css);
        }
        #endregion

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            Css css = e.Css.SingleOrDefault(x => x.Id == id);

            System.IO.File.Delete(GetCssFilePath(css));

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

        #region tools

        private string GetCssFilePath(Css model)
        {
            string applicationCSSPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Content\\userCSS\\";
            return applicationCSSPath + model.Name + ".css";
        }

        #endregion tools
    }
}