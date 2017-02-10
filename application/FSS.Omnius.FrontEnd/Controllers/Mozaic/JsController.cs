using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;
using System.Web.Helpers;
using System.IO;

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
            DBEntities e = DBEntities.instance;

            List<SelectListItem> appList = new List<SelectListItem>();
            foreach (Application app in e.Applications) {
                appList.Add(new SelectListItem() { Value = app.Id.ToString(), Text = app.Name, Selected = js == null ? false : (js.ApplicationId == app.Id) });
            }

            List<SelectListItem> pageList = new List<SelectListItem>();
            if (js != null && js.Application != null) {
                foreach (MozaicBootstrapPage page in js.Application.MozaicBootstrapPages) {
                    pageList.Add(new SelectListItem() { Value = page.Id.ToString(), Text = page.Name, Selected = js.MozaicBootstrapPageId == page.Id });
                }
            }

            ViewData["appList"] = appList;
            ViewData["pageList"] = pageList;

            return PartialView("Update", js);
        }

        // Save updates
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Js model)
        {
            DBEntities e = DBEntities.instance;

            if (string.IsNullOrWhiteSpace(model.Name)) {
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
            newJs.Name = "NewJS";
            newJs.Value = "";

            return SetUpdateJs(newJs);
        }

        public ActionResult GetPageList(int appId)
        {
            DBEntities e = DBEntities.instance;

            List<Dictionary<string, string>> pages = new List<Dictionary<string, string>>();
            foreach (MozaicBootstrapPage page in e.Applications.Single(a => a.Id == appId).MozaicBootstrapPages) {
                Dictionary<string, string> item = new Dictionary<string, string>();
                item.Add("Id", page.Id.ToString());
                item.Add("Name", page.Name);
                pages.Add(item);
            }
            
            return Json(pages);
        }

        #region tools

        private string GetJsFilePath(Js model)
        {
            DBEntities e = DBEntities.instance;
            Application app = e.Applications.Single(a => a.Id == model.ApplicationId);

            string applicationJSPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Scripts\\UserScripts\\Application\\" + app.Name;

            if(!Directory.Exists(applicationJSPath)) {
                Directory.CreateDirectory(applicationJSPath);
            }

            return applicationJSPath + "\\" + model.Name + ".js";
        }

        #endregion tools
    }
}