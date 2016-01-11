using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Hermes;

namespace FSS.Omnius.Controllers.Hermes
{
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
    }
}