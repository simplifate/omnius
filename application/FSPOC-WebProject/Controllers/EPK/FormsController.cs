using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class FormsController : Controller
    {
        // GET: Forms
        public ActionResult Index()
        {
            return View();
        }
    }
}