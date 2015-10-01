using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mozaic.Models;

namespace Mozaic.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int id)
        {
            DBMozaic e = new DBMozaic();

            Page page = e.Pages.FirstOrDefault(p => p.Id == id);
            return View(page);
        }
    }
}