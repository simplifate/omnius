using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.EPK
{
    [PersonaAuthorize(AppId = 26)]
    public class WF_StatesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
        }
        public ActionResult Update()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Update(FormCollection fc)
        {
            return RedirectToRoute("EPK", new { controller = "WF_States", action = "Index" });
        }
    }
}