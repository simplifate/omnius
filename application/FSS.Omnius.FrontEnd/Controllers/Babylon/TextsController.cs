using System.Web.Mvc;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Data.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using i18n.Domain.Concrete;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FSS.Omnius.Controllers.Babylon
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Babylon")]
    public class TextsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate()
        {
            string fileName = "messages.pot";

            DBEntities db = DBEntities.instance;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Page row in db.Pages) {
                data.Add(row.Id.ToString() + "-" + row.ViewName, row.ViewContent);
            }

            var settings = new i18nSettings(new WebConfigSettingService());
            var repository = new POTranslationRepository(settings);

            var nuggetFinder = new StringNuggetFinder(settings, data);
            var items = nuggetFinder.ParseAll();

            byte[] potContent = repository.GetTemplateForDownload(items);
                
            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(potContent);
            Response.End();
            
            return View("Index");
        }
        public ActionResult GeneratePot(int id)
        {
            string fileName = "messages.pot";

            DBEntities db = DBEntities.instance;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Page row in db.Pages)
            {
                if(row.ViewPath.ToString().StartsWith("/Views/App/") && Convert.ToInt32(row.ViewPath.ToString().Split('/')[3]) == id)
                data.Add(row.Id.ToString() + "-" + row.ViewName, row.ViewContent);
            }

            var settings = new i18nSettings(new WebConfigSettingService());
            var repository = new POTranslationRepository(settings);

            var nuggetFinder = new StringNuggetFinder(settings, data);
            var items = nuggetFinder.ParseAll();

            byte[] potContent = repository.GetTemplateForDownload(items);

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(potContent);
            Response.End();

            return View("Index");
        }
    }
}
