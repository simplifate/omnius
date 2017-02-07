using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using NSass;
using System;

namespace FSS.Omnius.Controllers.Mozaic
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
    public class JsController : Controller
    {
        // GET: Js
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;

            return View(e.Js);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            Js js = e.Js.SingleOrDefault(x => x.Id == id);

            return View(js);
        }

        #region Updates
        // Start update environment
        public ActionResult Update(int id)
        {
            DBEntities e = DBEntities.instance;
            Js js = e.Js.SingleOrDefault(x => x.Id == id);
            
            return SetUpdateJs(js);
        }
        
        public ActionResult SetUpdateJs(Js js)
        {
            ViewBag.JsValue = js.Value;
            ViewBag.JsName = js.Name;
            ViewBag.JsApplicationId = js.ApplicationId;

            return PartialView("Update", js);
        }

        // Save updates
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Js model)
        {
            DBEntities e = DBEntities.instance;

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return View("Update", model);
            }

            e.Js.AddOrUpdate(model);
            e.SaveChanges();

            //SassCompiler compiler = new SassCompiler();
            //var cssText = compiler.Compile(model.Value, OutputStyle.Compressed, true);

            string path = GetJsFilePath(model);
            System.IO.File.WriteAllText(path, model.Value);

            return View("BaseIndex", e.Js);
        }
        #endregion

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            Js js = e.Js.SingleOrDefault(x => x.Id == id);

            System.IO.File.Delete(GetJsFilePath(js));

            e.Js.Remove(js);
            e.SaveChanges();
            
            return RedirectToAction("Index");
        }

        //Create is like Update - but it is not taking CSS file from DB, it creates new
        
        public ActionResult Create()
        {
            Js newJs = new Js();
            newJs.Name = "New JS";
            newJs.Value = "";

            return SetUpdateJs(newJs);
        }

        #region tools

        private string GetJsFilePath(Js model)
        {
            string applicationJSPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Scripts\\UserScripts\\Application";
            return applicationJSPath + model.Name + ".js";
        }

        #endregion tools
    }
}