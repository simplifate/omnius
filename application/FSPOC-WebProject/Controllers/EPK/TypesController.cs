using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class TypesController : Controller
    {
        public ActionResult Index()
        {
            return View("/Views/App/26/Page/21.cshtml");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
        }
        public ActionResult Update()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Update(FormCollection fc)
        {
            return RedirectToRoute("EPK", new { controller = "Types", action = "Index" });
        }
    }
}