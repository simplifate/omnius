using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            return View();
        }
        public ActionResult UserNotAuthorized()
        {
            return View();
        }
        public ActionResult InternalServerError()
        {
            ViewData["exception"] = HttpContext.Items["exception"];
            HttpContext.Items["exception"] = null;
            return View();
        }
    }
}