using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Hermes;

namespace FSS.Omnius.Controllers.Hermes
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
    public class QueueController : Controller
    {
        // GET: Queue
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            return View(e.EmailQueueItems);
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            
            return PartialView("~/Views/Hermes/Queue/Detail.cshtml", e.EmailQueueItems.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            EmailQueue row = e.EmailQueueItems.Single(m => m.Id == id);

            e.EmailQueueItems.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        public ActionResult RunSender()
        {
            Mailer mailer = new Mailer("Test");
            mailer.RunSender();

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }
    }
}