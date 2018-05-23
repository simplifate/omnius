using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System.Linq;
using System.Web.Mvc;

namespace FSS.Omnius.FrontEnd.Controllers.Cortex
{
    public class CrontabController : Controller
    {
        public ActionResult Index()
        {
            return View(DBEntities.instance.CrontabTask.Where(ct => !ct.IsDeleted));
        }

        [HttpGet]
        public ActionResult Create()
        {
            var context = DBEntities.instance;
            ViewData["apps"] = context.Applications.Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString() });
            return View(new CrontabTask());
        }
        [HttpPost]
        public ActionResult Create(CrontabTask crontabTask)
        {
            var context = DBEntities.instance;

            if (!ModelState.IsValid)
            {
                ViewData["apps"] = context.Applications.Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString() });
                return View(crontabTask);
            }

            context.CrontabTask.Add(crontabTask);
            context.SaveChanges();
            crontabTask.Start();

            return RedirectToRoute("Cortex", new { Controller = "Crontab", Action = "Index" });
        }

        [HttpGet]
        public ActionResult Update(int Id)
        {
            var context = DBEntities.instance;
            CrontabTask crontabTask = context.CrontabTask.Find(Id);
            ViewData["apps"] = context.Applications.Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString(), Selected = app.Id == crontabTask.ApplicationId });
            return View(crontabTask);
        }
        [HttpPost]
        public ActionResult Update(CrontabTask crontabTask)
        {
            var context = DBEntities.instance;

            if (!ModelState.IsValid)
            {
                ViewData["apps"] = context.Applications.Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString(), Selected = app.Id == crontabTask.ApplicationId });
                return View(crontabTask);
            }
            
            context.CrontabTask.Find(crontabTask.Id).CopyPropertiesFrom(crontabTask);
            context.SaveChanges();
            crontabTask.End();
            crontabTask.Start();

            return RedirectToRoute("Cortex", new { Controller = "Crontab", Action = "Index" });
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var context = DBEntities.instance;
            CrontabTask crontabTask = context.CrontabTask.Find(Id);
            crontabTask.End();
            crontabTask.IsDeleted = true;
            crontabTask.IsActive = false;
            context.SaveChanges();

            return RedirectToRoute("Cortex", new { Controller = "Crontab", Action = "Index" });
        }
    }
}