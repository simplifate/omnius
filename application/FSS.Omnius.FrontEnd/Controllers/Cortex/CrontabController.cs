using FSS.Omnius.Modules.CORE;
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
            var allTasks = COREobject.i.Context.CrontabTask.Where(ct => !ct.IsDeleted);
            return View(allTasks);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var context = COREobject.i.Context;
            ViewData["apps"] = context.Applications.Select(app => new { app.DisplayName, app.Name, app.Id }).ToList().Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString() });
            return View(new CrontabTask());
        }
        [HttpPost]
        public ActionResult Create(CrontabTask crontabTask)
        {
            var context = COREobject.i.Context;

            if (!ModelState.IsValid)
            {
                ViewData["apps"] = context.Applications.Select(app => new { app.DisplayName, app.Name, app.Id }).ToList().Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString() });
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
            var context = COREobject.i.Context;
            CrontabTask crontabTask = context.CrontabTask.Find(Id);
            ViewData["apps"] = context.Applications.Select(app => new { app.DisplayName, app.Name, app.Id }).ToList().Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString(), Selected = app.Id == crontabTask.ApplicationId });
            return View(crontabTask);
        }
        [HttpPost]
        public ActionResult Update(CrontabTask crontabTask)
        {
            var context = COREobject.i.Context;

            if (!ModelState.IsValid)
            {
                ViewData["apps"] = context.Applications.Select(app => new { app.DisplayName, app.Name, app.Id }).ToList().Select(app => new SelectListItem { Text = app.DisplayName ?? app.Name, Value = app.Id.ToString(), Selected = app.Id == crontabTask.ApplicationId });
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
            var context = COREobject.i.Context;
            CrontabTask crontabTask = context.CrontabTask.Find(Id);
            crontabTask.End();
            crontabTask.IsDeleted = true;
            crontabTask.IsActive = false;
            context.SaveChanges();

            return RedirectToRoute("Cortex", new { Controller = "Crontab", Action = "Index" });
        }
    }
}
