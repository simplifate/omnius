using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.EPK
{
    [PersonaAuthorize]
    public class Heap_OrdersController : Controller
    {
        // GET: Heap_Orders
        public ActionResult Index()
        {
            HttpContext.SetApp(26);
            return View();
        }
    }
}