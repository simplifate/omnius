using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mozaic.Controllers
{
    [FilterIP(allowedIp = "127.0.0.1;::1")]
    public class ConfigController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}