using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class PeriodicalsController : Controller
    {
        // GET: Periodicals
        public ActionResult Index()
        {
            return View();
        }
    }
}